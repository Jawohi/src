using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Parcel
    {
        /// <summary>
        /// The tracking ID of the parcel. 
        /// </summary>
        /// <value>The tracking ID of the parcel. </value>
        public string TrackingId { get; set; }

        /// <summary>
        /// Gets or Sets Weight
        /// </summary>
        public float? Weight { get; set; }

        /// <summary>
        /// Gets or Sets Recipient
        /// </summary>
        public Recipient Recipient { get; set; }

        /// <summary>
        /// Gets or Sets Sender
        /// </summary>
        public Recipient Sender { get; set; }

        /// <summary>
        /// State of the parcel.
        /// </summary>
        /// <value>State of the parcel.</value>
        public enum StateEnum
        {
            PickupEnum = 0,
            InTransportEnum = 1,
            InTruckDeliveryEnum = 2,
            TransferredEnum = 3,
            DeliveredEnum = 4        
        }
        /// <summary>
        /// State of the parcel.
        /// </summary>
        /// <value>State of the parcel.</value>
        public StateEnum? State { get; set; }

        public DateTime? EntryDate { get; set; }

        /// <summary>
        /// Hops
        /// </summary>
        /// <value>Hops</value>
        public List<HopArrival> Hops { get; set; }
    }
}
