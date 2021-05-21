using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTerminal
{
    class Trade
    {
        public double Shares;
        public double Price;
        public DateTime Time;
        public Trade(string price, string shares, string time)
        {
            this.Shares = double.Parse(shares);
            this.Price = double.Parse(price.Substring(1));
            this.Time = DateTime.Parse(time);
        }
    }
}
