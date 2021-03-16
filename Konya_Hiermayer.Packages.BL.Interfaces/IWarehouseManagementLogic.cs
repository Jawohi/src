using Konya_Hiermayer.Packages.BL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL.Interfaces
{
    public interface IWarehouseManagementLogic
    {
        Warehouse ExportWarehouse();
        Warehouse ExportWarehouseCode(string code);

        void ImportWarehouse(Warehouse warehouse);
    }
}
