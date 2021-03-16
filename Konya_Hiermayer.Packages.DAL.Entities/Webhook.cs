using System;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Webhook
    { 
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Gets or Sets TrackingId
        /// </summary>
        public string TrackingId { get; set; }

        /// <summary>
        /// Gets or Sets Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or Sets CreatedAt
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }
}
