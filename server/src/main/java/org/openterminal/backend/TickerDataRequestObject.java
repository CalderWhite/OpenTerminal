package org.openterminal.backend;

public class TickerDataRequestObject {
    private String ticker;

    public TickerDataRequestObject() {}

    public TickerDataRequestObject(String ticker) {
        super();
        this.ticker = ticker;
    }

    public String getTicker() {
        return ticker;
    }

    public void setTicker(String ticker) {
        this.ticker = ticker;
    }
}
