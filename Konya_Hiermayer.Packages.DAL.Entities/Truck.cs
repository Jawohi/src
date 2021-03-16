using System;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

using System.ComponentModel.DataAnnotations.Schema;

namespace Konya_Hiermayer.Packages.DAL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Truck : Hop
    {
        [DataMember(Name = "regionGeoJson")]
        [Column(TypeName = "Geometry")]
        public Geometry RegionGeoJson { get; set; }

        public string NumberPlate { get; set; }
    }
}
