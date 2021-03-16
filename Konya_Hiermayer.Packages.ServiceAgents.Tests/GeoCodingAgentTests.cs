using Geocoding;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Konya_Hiermayer.Packages.ServiceAgents.Tests
{
	public class GeoCodingAgentTests
	{
		[Theory]
		[InlineData("Favoritenstraße 100, 1100 Wien", 48.1799961, 16.3756776)]
        [InlineData("Kärntnerstraße 11, 1010 Wien", 48.2069557, 16.37193131331169)]
        [InlineData("Hauptstraße 1, 7152 Pamhagen", 47.6974927, 16.9102791)]
        public void EncodeAddress_IsOK(string address, double latitude, double longitude)
		{
			IGeoCodingAgent agent = new OSMGeoCodingAgent();
			Location location = agent.EncodeAddress(address);
			Assert.Equal(latitude, location.Latitude);
			Assert.Equal(longitude, location.Longitude);
		}

		[Theory]
		[InlineData("Randomstree 99, 6666 Randomcity")]
		public void EncodeAddress_Fails(string address)
		{
			IGeoCodingAgent agent = new OSMGeoCodingAgent();
			Location location = agent.EncodeAddress(address);
			Assert.Null(location);
		}
	}
}