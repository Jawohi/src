using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.DAL.Interfaces
{
    public interface IHopRepository
    {
        void Create(Hop hop);

        Hop Read();

        void Update(Hop hop);

        void Delete();

        Hop GetByCode(string code);

        //Todo: entfernen
        Hop GetByCoordinates(IPoint point);
         Hop GetByCoordinates(double X, double Y);

    }
}
