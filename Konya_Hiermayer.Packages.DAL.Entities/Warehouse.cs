using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Warehouse : Hop
    {
        public IEnumerable<WarehouseNextHops> NextHops { get; set; }

        public Geometry RegionGeoJson { get; set; }
    }
}
