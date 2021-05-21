using System;
using System.Collections.Generic;

class MainClass {
  public static void Main (string[] args) {
    BaseConnector connector = new BaseConnector("AAPL", 100);
    {
        List<Trade> trades = new List<Trade>();
        trades.Add(new Trade("$126.35", "100", "10:59:55"));
        trades.Add(new Trade("$126.35", "100", "10:59:55"));
        trades.Add(new Trade("$126.3401", "1,000", "10:59:54"));
        trades.Add(new Trade("$126.34", "400", "10:59:54"));
        trades.Add(new Trade("$126.34", "100", "10:59:54"));
        trades.Add(new Trade("$126.34", "100", "10:59:54"));
        trades.Add(new Trade("$126.3397", "100", "10:59:54"));
        trades.Add(new Trade("$126.34", "100", "10:59:54"));
        trades.Add(new Trade("$126.3399", "100", "10:59:53"));
        trades.Add(new Trade("$126.33", "147", "10:59:53"));
        trades.Add(new Trade("$126.33", "100", "10:59:51"));
        trades.Add(new Trade("$126.325", "100", "10:59:51"));
        trades.Add(new Trade("$126.33", "100", "10:59:51"));
        trades.Add(new Trade("$126.33", "100", "10:59:51"));
        trades.Add(new Trade("$126.33", "200", "10:59:51"));
        trades.Add(new Trade("$126.3314", "400", "10:59:50"));
        trades.Add(new Trade("$126.33", "100", "10:59:49"));
        trades.Add(new Trade("$126.33", "100", "10:59:49"));
        trades.Add(new Trade("$126.32", "100", "10:59:48"));
        trades.Add(new Trade("$126.325", "200", "10:59:48"));

        connector.DedupeTrades(trades);
    }


    connector.PrintStuff();
    Console.WriteLine("------------------------------------");

    List<Trade> _trades;

    {
        List<Trade> trades = new List<Trade>();

        // new data
        trades.Add(new Trade("$126.3602", "100", "10:59:58"));
        trades.Add(new Trade("$126.36", "100", "10:59:58"));
        trades.Add(new Trade("$126.36", "100", "10:59:58"));
        trades.Add(new Trade("$126.3676", "300", "10:59:58"));
        trades.Add(new Trade("$126.36", "100", "10:59:57"));
        trades.Add(new Trade("$126.36", "100", "10:59:57"));
        trades.Add(new Trade("$126.36", "172", "10:59:57"));
        trades.Add(new Trade("$126.36", "147", "10:59:57"));
        trades.Add(new Trade("$126.36", "153", "10:59:57"));
        trades.Add(new Trade("$126.3501", "3,000", "10:59:56"));
        trades.Add(new Trade("$126.355", "100", "10:59:56"));
        trades.Add(new Trade("$126.35", "100", "10:59:55"));

        // old data
        trades.Add(new Trade("$126.35", "100", "10:59:55"));
        trades.Add(new Trade("$126.35", "100", "10:59:55"));
        trades.Add(new Trade("$126.3401", "1,000", "10:59:54"));
        trades.Add(new Trade("$126.34", "400", "10:59:54"));
        trades.Add(new Trade("$126.34", "100", "10:59:54"));
        trades.Add(new Trade("$126.34", "100", "10:59:54"));
        trades.Add(new Trade("$126.3397", "100", "10:59:54"));
        trades.Add(new Trade("$126.34", "100", "10:59:54"));

        connector.DedupeTrades(trades);

        _trades = trades;
    }

    connector.PrintStuff();
    Console.WriteLine("-------------------------------------*");

    _trades.ForEach(Console.WriteLine);
  }
}