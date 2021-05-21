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

        public bool Equals(Trade t)
        {
            return this.Shares.Equals(t.Shares) && this.Price.Equals(t.Price) && this.Time.Equals(t.Time);
        }

        public override string ToString()
        {
            return String.Format("${0:N3}\t{1,5}\t{2}", this.Price, this.Shares, this.Time.ToString());
        }
    }
}
