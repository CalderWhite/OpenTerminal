package org.openterminal.backend;

import java.util.ArrayList;

public class Ticker {
    private final String ticker;
    private final ArrayList<TradeObject> prevTrades;

    Ticker(String ticker) {
        this.ticker = ticker;
        this.prevTrades = new ArrayList<>();
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
