package org.openterminal.backend;

import java.util.ArrayList;

public class TradeListObject {
    private String ticker;
    private ArrayList<TradeObject> trades;

    public TradeListObject() {}

    public TradeListObject(String ticker, ArrayList<TradeObject> trades) {
        super();
        this.ticker = ticker;
        this.trades = trades;
    }

    public String getTicker() {
        return ticker;
    }

    public void setTicker(String ticker) {
        this.ticker = ticker;
    }

    public ArrayList<TradeObject> getTrades() {
        return trades;
    }

    public void setTrades(ArrayList<TradeObject> trades) {
        this.trades = trades;
    }
}
