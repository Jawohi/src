using System;
using System.ComponentModel.DataAnnotations;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 

    public partial class WarehouseNextHops
    {
        [Key]
        public int ID {get;set;}
        public string FirstHopId { get; set; }
        public Warehouse FirstHop { get; set; }

        /// <summary>
        /// Gets or Sets TraveltimeMins
        /// </summary>
        public int? TraveltimeMins { get; set; }

        /// <summary>
        /// Gets or Sets Hop
        /// </summary>
        public string SecondHopId { get; set; }
        public Hop SecondHop { get; set; }
    }
}
