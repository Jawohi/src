using Konya_Hiermayer.Packages.BL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL.Interfaces
{
    public interface IRecipientLogic
    {

        Parcel TrackParcel(string parcelID);

    }
}
