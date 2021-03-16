using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL.Interfaces
{
    public interface IStaffLogic
    {
        void ReportParcelDelivery(string parcelID);

        void ReportParcelHop(string parcelID, string code);
    }
}
