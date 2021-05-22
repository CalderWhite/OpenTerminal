using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Threading;

namespace OpenTerminal
{
    class DummyConnector : BaseConnector
    {
        public DummyConnector(string ticker) : base(ticker) { }

        public override List<Trade> GetLatestTrades()
        {
            string url = String.Format("http://127.0.0.1:5000/api/quote/{0}/realtime-trades?limit=100", this.ticker);
            var client = new HttpClient();
            var task = Task.Run(() => client.GetStringAsync(url));
            task.Wait();
            var res = task.Result;
            var jsonTask = Task.Run(() => JsonSerializer.Deserialize<Rootobject>(res));
            jsonTask.Wait();
            var json = jsonTask.Result;

            List<Trade> trades = new List<Trade>();
            for (int i=0; i<json.data.rows.Length; ++i)
            {
                var row = json.data.rows[i];
                trades.Add(new Trade(row.nlsPrice, row.nlsShareVolume, row.nlsTime));
            }

            DedupeTrades(trades);

            return trades;
        }
    }
}
