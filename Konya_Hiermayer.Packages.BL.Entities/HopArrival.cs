using System;
using Date = System.DateTime;

namespace Konya_Hiermayer.Packages.BL.Entities
{
    public partial class HopArrival
    {
        public Hop Hop { get; set; }

        /// <summary>
        /// The date/time the parcel arrived at the hop.
        /// </summary>
        /// <value>The date/time the parcel arrived at the hop.</value>
        public DateTime? DateTime { get; set; }

        public HopArrival(Hop hop)
        {
            Hop = hop;
            DateTime = Date.Now;
        }

        public HopArrival(Hop hop, DateTime dateTime)
        {
            this.Hop = hop;
            this.DateTime = dateTime;
        }
    }
}