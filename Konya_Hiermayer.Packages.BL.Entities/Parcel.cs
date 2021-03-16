using System;

using System.Collections.Generic;

namespace Konya_Hiermayer.Packages.BL.Entities
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

        public DateTime? EntryDate {get; set;}

        /// <summary>
        /// Hops visited in the past.
        /// </summary>
        /// <value>Hops visited in the past.</value>
        public List<HopArrival> VisitedHops { get; set; }

        /// <summary>
        /// Hops coming up in the future - their times are estimations.
        /// </summary>
        /// <value>Hops coming up in the future - their times are estimations.</value>
        public List<HopArrival> FutureHops { get; set; }

        public void ReportHop(Hop visitedHop)
        {
            switch(visitedHop)
            {
                case Truck t:
                    {
                        State = StateEnum.InTruckDeliveryEnum;
                        break;
                    }
                case Transferwarehouse wh:
                    {
                        State = StateEnum.TransferredEnum;
                        break;
                    }
                case Warehouse w:
                    {
                        State = StateEnum.InTransportEnum;
                        break;
                    }
            }
        }

        public void ReportDelivery()
        {
            State = StateEnum.DeliveredEnum;
        }

        public void Submit(string newTrackingId)
        {
            TrackingId = newTrackingId;
            EntryDate = DateTime.Now;
        }

    }
}
