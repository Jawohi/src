using GeoAPI.Geometries;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL.Extensions
{
    public static class LocationExtensions
    {
        public static IPoint ToGeoPoint(this Geocoding.Location location)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var currentLocation = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(location.Longitude, location.Latitude));
            return (IPoint)currentLocation;

        }
    }
}
