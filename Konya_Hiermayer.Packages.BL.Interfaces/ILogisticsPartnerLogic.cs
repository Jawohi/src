using Konya_Hiermayer.Packages.BL.Entities;
using System;

namespace Konya_Hiermayer.Packages.BL.Interfaces
{
    public interface ILogisticsPartnerLogic
    {
        string TransferParcel(string parcelID, Parcel parcel);
    }
}
