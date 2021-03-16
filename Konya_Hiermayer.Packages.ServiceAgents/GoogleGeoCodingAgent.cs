using Geocoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nominatim.API.Geocoders;
using Nominatim.API.Models;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;

namespace Konya_Hiermayer.Packages.ServiceAgents
{
    public class OSMGeoCodingAgent : IGeoCodingAgent
    {
        public Location EncodeAddress(string address)
        {
            var geocoder = new ForwardGeocoder();

            var request = geocoder.Geocode(new ForwardGeocodeRequest
            {
                queryString = address,

                BreakdownAddressElements = true,
                ShowExtraTags = true,
                ShowAlternativeNames = true,
                ShowGeoJSON = true
            });
            request.Wait();

            var result = request.Result.FirstOrDefault();
            if (result == null) return null;
            Location location = new Location(result.Latitude, result.Longitude);
            return location;
        }
    }
}
