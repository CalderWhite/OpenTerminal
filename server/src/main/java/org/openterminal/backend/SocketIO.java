package org.openterminal.backend;

import com.corundumstudio.socketio.AckRequest;
import com.corundumstudio.socketio.Configuration;
import com.corundumstudio.socketio.SocketIOClient;
import com.corundumstudio.socketio.SocketIOServer;
import com.corundumstudio.socketio.listener.ConnectListener;
import com.corundumstudio.socketio.listener.DataListener;
import com.corundumstudio.socketio.listener.DisconnectListener;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.util.*;

public class SocketIO {
    private final SocketIOServer server;
    private final HashMap<String, Ticker> tickers;
    private final HashMap<UUID, String> clientTickerMap;
    private boolean logData = false;

    public SocketIO(Configuration config) {
        this.tickers = new HashMap<>();
        this.server = new SocketIOServer(config);
        this.clientTickerMap = new HashMap<>();

        // Handle on-connect
        this.server.addConnectListener(new ConnectListener() {
            @Override
            public void onConnect(SocketIOClient socketIOClient) {
                String requestedTicker = socketIOClient.getHandshakeData().getSingleUrlParam("ticker");
                if (logData) {
                    System.out.println("Client requested connection to ticker "+requestedTicker+".");
                }

                // If the ticker isn't being actively tracked, create a new Ticker class and add it to the dict.
                boolean newlyCreated = false;
                if (!tickers.containsKey(requestedTicker)) {
                    tickers.put(requestedTicker, new Ticker(requestedTicker));
                    newlyCreated = true;
                }

                Ticker tickerObject = tickers.get(requestedTicker);
                try {
                    tickerObject.registerNewClient(socketIOClient);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                if (newlyCreated) {
                    tickerObject.startLoop();
                }

                clientTickerMap.put(socketIOClient.getSessionId(), requestedTicker);
            }
        });

        this.server.addDisconnectListener(new DisconnectListener() {
            @Override
            public void onDisconnect(SocketIOClient socketIOClient) {
                UUID clientUUID = socketIOClient.getSessionId();
                String requestedTicker = clientTickerMap.get(clientUUID);
                clientTickerMap.remove(clientUUID);

                if (logData) {
                    System.out.println("Client requested disconnect from ticker "+requestedTicker+".");
                }

                Ticker tickerObject = tickers.get(requestedTicker);
                if (tickerObject.dropClient(socketIOClient)) {
                    tickers.remove(requestedTicker);
                }
            }
        });

        // Create a new listener on the push-trades event, which expects a TradeListObject.
        this.server.addEventListener("push-trades", TradeListObject.class, new DataListener<TradeListObject>() {
            @Override
            public void onData(SocketIOClient socketIOClient, TradeListObject tradeListObject, AckRequest ackRequest) throws Exception {
                String ticker = tradeListObject.getTicker();

                // Debug incoming data.
                if (logData) {
                    debugData(tradeListObject);
                }

                // Dedupe the trades.
                Ticker tickerObj = tickers.get(ticker);
                ArrayList<TradeObject> trades = tradeListObject.getTrades();
                tickerObj.dedupeTrades(trades);

                // Broadcast deduped trades to the ticker room.
                server.getBroadcastOperations().sendEvent(ticker, new TradeListObject(ticker, trades));

                // TODO: push to db
            }
        });
    }

    public void start() {
        this.server.start();
    }

    public void stop() {
        for (Map.Entry<UUID, String> entry : clientTickerMap.entrySet()) {
            this.tickers.get(entry.getValue()).dropClient(this.server.getClient(entry.getKey()));
        }

        this.server.stop();
    }

    private void debugData(TradeListObject data) throws JsonProcessingException {
        ObjectMapper om = new ObjectMapper();
        System.out.println(om.writeValueAsString(data));
    }

    public void enableDataLogging() {
        this.logData = true;
    }

    public void disableDataLogging() {
        this.logData = false;
    }
}
