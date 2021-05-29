using System;
using System.Collections.Generic;

namespace OpenTerminal
{
    abstract class BaseConnector
    {
        // the ticker we are subscribing to.
        protected string ticker;
        // tracks the last list of trades for deduping purposes. Sorted in descending order (First element is the newest)
        private List<Trade> prevTrades = new List<Trade>();

        public BaseConnector(string ticker)
        {
            this.ticker = ticker;
        }

        /**
         * This function removes duplicate data from [tradeList] by saving the last incoming list of trades
         * in a class member.
         * 
         * @param tradeList A list of Trade objects that is sorted in descending order by Time. (The first element is the newest, the last is the oldest)
         * 
         * Extended explanation:
         *     Unless the security is doing > 1 page of data per second (so for the nasdaq, that's 100 trades per second),
         *     there will be overlap in the data the connector recieves and the data it has already seen.
         */
        public void DedupeTrades(List<Trade> tradeList)
        {
            bool foundMatch = false;
            int matchCount = 0;

            // we are aiming to find a subset of tradeList within prevTrades
            // we look from the back because those are the oldest trades which we are most sure should be deleted
            // whereas if you started from the front, you don't know how many new trades there are
            for (int i = tradeList.Count - 1; i >= 0; --i)
            {
                for (int j = prevTrades.Count - 1; j >= 0; --j)
                {
                    // if we find a match, we know this is the oldest and it is where we should start
                    // because these lists are sorted in temporal order
                    if (tradeList[i].Equals(prevTrades[j]))
                    {
                        foundMatch = true;

                        ++matchCount;
                        --i;
                    }
                    else if (foundMatch)
                    {
                        goto Done;
                    }
                }

                if (foundMatch)
                {
                    goto Done;
                }
            }

        Done:

            prevTrades.Clear();
            prevTrades.AddRange(tradeList);

            tradeList.RemoveRange(tradeList.Count - matchCount, matchCount);
        }

        /**
          * It is up to the Connector classes to manage the intervals the actually get data from their API and
          * that they pull data from out centralized server. The MainWindow doesn't care about that. It just wants
          * fresh data at a fixed interval, provided by GetLatestTrades();
          */
        abstract public List<Trade> GetLatestTrades();
    }
}