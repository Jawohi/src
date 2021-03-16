using System;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class HopArrival
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParcelId { get; set; }
        public Parcel Parcel { get; set; }

        /// <summary>
        /// Unique CODE of the hop.
        /// </summary>
        /// <value>Unique CODE of the hop.</value>
        /// Hop the Parcel arrived at.
        /// </summary>
        public string HopId { get; set; }
        public Hop Hop { get; set; }

        /// <summary>
        /// The date/time the parcel arrived at the hop.
        /// </summary>
        /// <value>The date/time the parcel arrived at the hop.</value>
        public DateTime? DateTime { get; set; }

        public bool WasVisited { get; set; } = false;
    }
}