using System;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 

    public abstract class Hop
    {
        /// <summary>
        /// Unique CODE of the hop.
        /// </summary>
        /// <value>Unique CODE of the hop.</value>
        public string Code { get; set; }

        /// <summary>
        /// Description of the hop.
        /// </summary>
        /// <value>Description of the hop.</value>
        public string Description { get; set; }

        /// <summary>
        /// Delay processing takes on the hop.
        /// </summary>
        /// <value>Delay processing takes on the hop.</value>
        public int? ProcessingDelayMins { get; set; }
        public double latitude;//{ get; set; }
        public double longitude;//{ get; set; }

    


    }
}
