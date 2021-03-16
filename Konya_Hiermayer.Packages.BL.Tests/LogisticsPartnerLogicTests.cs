using System;
using AutoMapper;
using Moq;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using DA = Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.Mapping;
using Xunit;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;
using Konya_Hiermayer.Packages.BL.Extensions;
using System.Collections.Generic;

namespace Konya_Hiermayer.Packages.BL.Tests
{
    public class LogisticsPartnerLogicTests : IDisposable
    {
        IMapper mapper;

        public LogisticsPartnerLogicTests()
        {
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            mapper = new Mapper(configuration);
        }

        public void Dispose()
        {

        }

        [Fact]
        public void TransferParcel_ValidParcel()
        {
            string trackingid = "ABCD12345";
            float weight = 1.0f;

            Recipient sender = new Recipient() { Name = "Martin", City = "Wien", Country = "Österreich", PostalCode = "1100", Street="Radnitzkygasse 16" };
            Recipient recipient = new Recipient() { Name = "Simon", City = "Wien", Country = "Österreich", PostalCode = "1100", Street="Suchenwirtplatz 10" };
            Parcel p = new Parcel() { Sender=sender, Recipient=recipient, Weight=weight, TrackingId=trackingid };

            // geoMock Setup
            Mock<IGeoCodingAgent> geoMock = new Mock<IGeoCodingAgent>();

            string senderGeoString = p.Sender.ToGeoCodingString();
            string recipientGeoString = p.Recipient.ToGeoCodingString();

            Geocoding.Location senderLoc = new Geocoding.Location(1, 1);
            Geocoding.Location recipientLoc = new Geocoding.Location(2, 2);

            geoMock.Setup(foo => foo.EncodeAddress(recipientGeoString)).Returns(recipientLoc);
            geoMock.Setup(foo => foo.EncodeAddress(senderGeoString)).Returns(senderLoc);

            //hopMock Setup
            Mock<IHopRepository> hopMock = new Mock<IHopRepository>();

            //var senderLoc.Latitude, senderLoc.Longitude = senderLoc.ToGeoPoint();
            //var recipientLoc.Latitude, recipientLoc.Longitude = recipientLoc.ToGeoPoint();

            DA.Hop senderHop = new DA.Warehouse() { Code = "ABCD", Description="senderHop" };
            DA.Hop recipientHop = new DA.Warehouse() { Code = "EFGH", Description="recipientHop" };

            hopMock.Setup(foo => foo.GetByCoordinates(senderLoc.Latitude, senderLoc.Longitude)).Returns(senderHop);
            hopMock.Setup(foo => foo.GetByCoordinates(recipientLoc.Latitude, recipientLoc.Longitude)).Returns(recipientHop);

            // wareMock Setup
            Mock<IWarehouseRepository> wareMock = new Mock<IWarehouseRepository>();
            DA.Warehouse fullWa = new DA.Warehouse() { Code = "A1B2", Description = "fullWa" };
            wareMock.Setup(foo => foo.Read()).Returns(fullWa);


            // routeMock Setup
            Mock<IRouteCalculator> routeMock = new Mock<IRouteCalculator>();

            Warehouse fullWaBusiness = this.mapper.Map<Warehouse>(fullWa);
            Hop senderHopBusiness = this.mapper.Map<Hop>(senderHop);
            Hop recipientHopBusiness = this.mapper.Map<Hop>(recipientHop);

            List<HopArrival> arrivals = new List<HopArrival>();
            arrivals.Add(new HopArrival(senderHopBusiness));
            arrivals.Add(new HopArrival(recipientHopBusiness));

            routeMock.Setup(foo => foo.CalculateRoute(fullWaBusiness, senderHop.Code, recipientHop.Code)).Returns(arrivals);

            // parcelMock Setup
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();
            parcelMock.Setup(foo => foo.Create(It.IsAny<DA.Parcel>()));

            ILogisticsPartnerLogic logisticsPartnerLogic = new LogisticsPartnerLogic(mapper, parcelMock.Object, wareMock.Object, hopMock.Object, geoMock.Object, routeMock.Object, NullLogger<LogisticsPartnerLogic>.Instance);

            string parcelID = logisticsPartnerLogic.TransferParcel(p.TrackingId, p);
            Assert.Equal(trackingid, parcelID);
        }

        [Theory]
        [InlineData("ABCD12", 1.0f)]
        [InlineData("ABCD12345", -1.0f)]
        [InlineData("ABCD12", -1.0f)]
        public void TransferParcel_InValidParcel(string trackingid, float weight)
        {
            Recipient sender = new Recipient() { Name = "Martin", City = "Wien", Country = "Österreich", PostalCode = "1100", Street = "Radnitzkygasse 16" };
            Recipient recipient = new Recipient() { Name = "Simon", City = "Wien", Country = "Österreich", PostalCode = "1100", Street = "Suchenwirtplatz 10" };
            Parcel p = new Parcel() { Sender = sender, Recipient = recipient, Weight = weight, TrackingId = trackingid };

            // geoMock Setup
            Mock<IGeoCodingAgent> geoMock = new Mock<IGeoCodingAgent>();

            string senderGeoString = p.Sender.ToGeoCodingString();
            string recipientGeoString = p.Recipient.ToGeoCodingString();

            Geocoding.Location senderLoc = new Geocoding.Location(1, 1);
            Geocoding.Location recipientLoc = new Geocoding.Location(2, 2);

            geoMock.Setup(foo => foo.EncodeAddress(recipientGeoString)).Returns(recipientLoc);
            geoMock.Setup(foo => foo.EncodeAddress(senderGeoString)).Returns(senderLoc);

            //hopMock Setup
            Mock<IHopRepository> hopMock = new Mock<IHopRepository>();

            //var senderLoc.Latitude, senderLoc.Longitude = senderLoc.ToGeoPoint();
            //var recipientLoc.Latitude, recipientLoc.Longitude = recipientLoc.ToGeoPoint();

            DA.Hop senderHop = new DA.Warehouse() { Code = "ABCD", Description = "senderHop" };
            DA.Hop recipientHop = new DA.Warehouse() { Code = "EFGH", Description = "recipientHop" };

            hopMock.Setup(foo => foo.GetByCoordinates(senderLoc.Latitude, senderLoc.Longitude)).Returns(senderHop);
            hopMock.Setup(foo => foo.GetByCoordinates(recipientLoc.Latitude, recipientLoc.Longitude)).Returns(recipientHop);

            // wareMock Setup
            Mock<IWarehouseRepository> wareMock = new Mock<IWarehouseRepository>();
            DA.Warehouse fullWa = new DA.Warehouse() { Code = "A1B2", Description = "fullWa" };
            wareMock.Setup(foo => foo.Read()).Returns(fullWa);


            // routeMock Setup
            Mock<IRouteCalculator> routeMock = new Mock<IRouteCalculator>();

            Warehouse fullWaBusiness = this.mapper.Map<Warehouse>(fullWa);
            Hop senderHopBusiness = this.mapper.Map<Hop>(senderHop);
            Hop recipientHopBusiness = this.mapper.Map<Hop>(recipientHop);

            List<HopArrival> arrivals = new List<HopArrival>();
            arrivals.Add(new HopArrival(senderHopBusiness));
            arrivals.Add(new HopArrival(recipientHopBusiness));

            routeMock.Setup(foo => foo.CalculateRoute(fullWaBusiness, senderHop.Code, recipientHop.Code)).Returns(arrivals);

            // parcelMock Setup
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();

            ILogisticsPartnerLogic logisticsPartnerLogic = new LogisticsPartnerLogic(mapper, parcelMock.Object, wareMock.Object, hopMock.Object, geoMock.Object, routeMock.Object, NullLogger<LogisticsPartnerLogic>.Instance);

            Assert.Throws<InvalidParcelException>(() => logisticsPartnerLogic.TransferParcel(p.TrackingId, p));
        }

        [Fact]
        public void TransferParcel_ThrowsNoHopException()
        {
            string trackingid = "ABCD12345";
            float weight = 1.0f;

            Recipient sender = new Recipient() { Name = "Martin", City = "Wien", Country = "Österreich", PostalCode = "1100", Street = "Radnitzkygasse 16" };
            Recipient recipient = new Recipient() { Name = "Simon", City = "Wien", Country = "Österreich", PostalCode = "1100", Street = "Suchenwirtplatz 10" };
            Parcel p = new Parcel() { Sender = sender, Recipient = recipient, Weight = weight, TrackingId = trackingid };

            // geoMock Setup
            Mock<IGeoCodingAgent> geoMock = new Mock<IGeoCodingAgent>();
            
            string senderGeoString = p.Sender.ToGeoCodingString();
            string recipientGeoString = p.Recipient.ToGeoCodingString();

            Geocoding.Location senderLoc = new Geocoding.Location(1, 1);
            Geocoding.Location recipientLoc = new Geocoding.Location(2, 2);
            

            geoMock.Setup(foo => foo.EncodeAddress(recipientGeoString)).Returns(recipientLoc);
            geoMock.Setup(foo => foo.EncodeAddress(senderGeoString)).Returns(senderLoc);

            //hopMock Setup
            Mock<IHopRepository> hopMock = new Mock<IHopRepository>();

            //var senderLoc.Latitude, senderLoc.Longitude = senderLoc.ToGeoPoint();
            //var recipientLoc.Latitude, recipientLoc.Longitude = recipientLoc.ToGeoPoint();

            DA.Hop senderHop = new DA.Warehouse() { Code = "ABCD", Description = "senderHop" };
            DA.Hop recipientHop = new DA.Warehouse() { Code = "EFGH", Description = "recipientHop" };

            hopMock.Setup(foo => foo.GetByCoordinates(senderLoc.Latitude, senderLoc.Longitude)).Returns((DA.Hop)null);
            hopMock.Setup(foo => foo.GetByCoordinates(recipientLoc.Latitude, recipientLoc.Longitude)).Returns((DA.Hop)null);

            // wareMock Setup
            Mock<IWarehouseRepository> wareMock = new Mock<IWarehouseRepository>();

            // routeMock Setup
            Mock<IRouteCalculator> routeMock = new Mock<IRouteCalculator>();

            // parcelMock Setup
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();

            ILogisticsPartnerLogic logisticsPartnerLogic = new LogisticsPartnerLogic(mapper, parcelMock.Object, wareMock.Object, hopMock.Object, geoMock.Object, routeMock.Object, NullLogger<LogisticsPartnerLogic>.Instance);

            Assert.Throws<NoHopException>(() => logisticsPartnerLogic.TransferParcel(p.TrackingId, p));
        }
    }
}
