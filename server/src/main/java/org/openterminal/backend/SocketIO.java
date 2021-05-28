package org.openterminal.backend;

import com.corundumstudio.socketio.AckRequest;
import com.corundumstudio.socketio.Configuration;
import com.corundumstudio.socketio.SocketIOClient;
import com.corundumstudio.socketio.SocketIOServer;
import com.corundumstudio.socketio.listener.DataListener;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.util.ArrayList;
import java.util.HashMap;

public class SocketIO {
    private final SocketIOServer server;
    private final HashMap<String, Ticker> tickers;
    private boolean logData = false;

    public SocketIO(Configuration config) {
        this.tickers = new HashMap<>();
        this.server = new SocketIOServer(config);

        // Create a new listener on the push-trades event, which expects a TradeListObject.
        this.server.addEventListener("push-trades", TradeListObject.class, new DataListener<TradeListObject>() {
            @Override
            public void onData(SocketIOClient socketIOClient, TradeListObject tradeListObject, AckRequest ackRequest) throws Exception {
                String ticker = tradeListObject.getTicker();

                // If the ticker isn't being actively tracked, create a new Ticker class and add it to the dict.
                if (!tickers.containsKey(ticker)) {
                    tickers.put(ticker, new Ticker(ticker));
                }

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
