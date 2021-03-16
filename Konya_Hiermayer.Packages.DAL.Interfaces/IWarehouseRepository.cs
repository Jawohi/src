using System;
using System.Collections.Generic;
using Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.DAL.Interfaces
{
    public interface IWarehouseRepository
    {
        void Create(Warehouse warehouse);

        Warehouse Read();

        void Update(Warehouse hop);

        void Delete();

        Warehouse GetByCode(string code);
    }
}
