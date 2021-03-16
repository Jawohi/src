using System;
using System.Collections.Generic;
using Konya_Hiermayer.Packages.BL.Entities;

namespace Konya_Hiermayer.Packages.BL.Interfaces
{
    public interface IRouteCalculator
    {
        List<HopArrival> CalculateRoute(Warehouse hierarchy, string codeSender, string codeRecipient, DateTime? EntryDate);
    }
}
