using System.Collections.Generic;


namespace Konya_Hiermayer.Packages.BL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Warehouse : Hop
    { 
        public IEnumerable<WarehouseNextHops> NextHops { get; set; }
        
    }
}
