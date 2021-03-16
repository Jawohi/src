using System;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Transferwarehouse : Hop
    {
        public Geometry RegionGeoJson { get; set; }

        public string LogisticsPartner { get; set; }

        public string LogisticsPartnerUrl { get; set; }
    }
}
