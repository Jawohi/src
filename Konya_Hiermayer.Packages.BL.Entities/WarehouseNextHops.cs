namespace Konya_Hiermayer.Packages.BL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class WarehouseNextHops
    { 
        /// <summary>
        /// Gets or Sets TraveltimeMins
        /// </summary>
        public int? TraveltimeMins { get; set; }

        /// <summary>
        /// Gets or Sets Hop
        /// </summary>
        public Hop Hop { get; set; }

        public string SecondHopId { get; set; }
    }
}
