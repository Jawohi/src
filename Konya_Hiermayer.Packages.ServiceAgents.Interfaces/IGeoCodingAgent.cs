using Geocoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Konya_Hiermayer.Packages.ServiceAgents.Interfaces
{
    public interface IGeoCodingAgent
    {
        Location EncodeAddress(string address);
    }
}
