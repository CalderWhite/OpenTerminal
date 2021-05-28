using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTerminal
{
    public class Trade
    {
        public double Volume { get; set; }
        public double Price { get; set; }
        public DateTime Time { get; set; }

        // internal. Not to be sent outside of this application
        // -1: lower
        //  0: same
        //  1: higher
        public int HigherThanLast { get; set; }

        public Trade(string price, string shares, string time)
        {
            this.Volume = double.Parse(shares);
            this.Price = double.Parse(price.Substring(1));
            this.Time = DateTime.Parse(time);
            this.HigherThanLast = 0;
        }

        public bool Equals(Trade t)
        {
            return this.Volume.Equals(t.Volume) && this.Price.Equals(t.Price) && this.Time.Equals(t.Time);
        }

        public override string ToString()
        {
            return String.Format("${0:N3}\t{1,5}\t{2}", this.Price, this.Volume, this.Time.ToString());
        }


    }
}
