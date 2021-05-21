using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTerminal {

    interface IExchangeConnector
    {
        /**
         * It is up to the Connector classes to manage the intervals the actually get data from their API and
         * that they pull data from out centralized server. The MainWindow doesn't care about that. It just wants
         * fresh data at a fixed interval, provided by GetLatestTrades();
         */
        List<Trade> GetLatestTrades();
    }
}
