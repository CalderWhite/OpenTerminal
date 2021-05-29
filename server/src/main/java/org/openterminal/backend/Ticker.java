package org.openterminal.backend;

import com.corundumstudio.socketio.SocketIOClient;

import java.util.*;
import java.util.concurrent.LinkedBlockingQueue;

public class Ticker {
    private final String ticker;
    private final ArrayList<TradeObject> prevTrades;
    private final LinkedBlockingQueue<SocketIOClient> connectedClients;
    private final Thread requestLoop;

    Ticker(String ticker) {
        this.ticker = ticker;
        this.prevTrades = new ArrayList<>();
        this.connectedClients = new LinkedBlockingQueue<>();

        this.requestLoop = new Thread(() -> {

            while (!connectedClients.isEmpty()) {
                SocketIOClient client = null;
                try {
                    client = connectedClients.take();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                client.sendEvent(this.ticker+"-request", new TickerDataRequestObject(this.ticker));

                try {
                    connectedClients.put(client);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                try {
                    Thread.sleep(15000/(connectedClients.size()));
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }

            System.out.println(this.ticker+" has no clients left, exiting..");
        });
    }

    public void registerNewClient(SocketIOClient client) throws InterruptedException {
        this.connectedClients.put(client);
    }

    public boolean dropClient(SocketIOClient client) {
        this.connectedClients.remove(client);
        return this.connectedClients.isEmpty();
    }

    public void startLoop() {
        System.out.println(this.ticker+" thread starting..");
        requestLoop.start();
    }

    public void joinThread() throws InterruptedException {
        requestLoop.join();
    }

    // https://github.com/CalderWhite/OpenTerminal/blob/master/windows-client/BaseConnector.cs#L28
    public void dedupeTrades(ArrayList<TradeObject> tradeList) {
        boolean foundMatch = false;
        int matchCount = 0;

        for (int i = tradeList.size() - 1; i >= 0; --i) {
            for (int j = this.prevTrades.size() - 1; j >= 0; --j) {
                if (tradeList.get(i).Equals(this.prevTrades.get(j))) {
                    foundMatch = true;
                    ++matchCount;
                    --i;
                }
                else if (foundMatch) {
                    break;
                }
            }

            if (foundMatch) {
                break;
            }
        }

        this.prevTrades.clear();
        this.prevTrades.addAll(tradeList);

        if (matchCount > 0) {
            tradeList.subList(tradeList.size() - matchCount, tradeList.size()).clear();
        }
    }

    public String getTicker() {
        return this.ticker;
    }
}
