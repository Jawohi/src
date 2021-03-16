using System;
using Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.DAL.Interfaces
{
    public interface IParcelRepository
    {
        void Create(Parcel parcel);

        void Delivered(Parcel parcel);

        void Update(Parcel parcel);

        void Delete();

        Parcel GetByTrackingId(string trackingId);
    }
}
