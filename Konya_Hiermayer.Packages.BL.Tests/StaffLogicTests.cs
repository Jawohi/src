using System;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using DA = Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Konya_Hiermayer.Packages.Mapping;
using Xunit;
using System.Collections.Generic;

namespace Konya_Hiermayer.Packages.BL.Tests
{
    public class StaffLogicTests : IDisposable
    {
        IMapper mapper;

        public StaffLogicTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            mapper = new Mapper(configuration);
        }

        public void Dispose()
        {

        }

        [Fact]
        public void ReportParcelDelivery_GetByTrackingIdExpection()
        {
            string trackingId = "123456789";

            Mock<IParcelRepository> parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(foo => foo.GetByTrackingId(trackingId)).Throws(new ParcelNotFoundExpection());

            Mock<IHopRepository> hopRepoMock = new Mock<IHopRepository>();
            Mock<IWebhookRepository> hookMock = new Mock<IWebhookRepository>();

            IStaffLogic staffLogic = new StaffLogic(mapper, parcelRepoMock.Object, hopRepoMock.Object, hookMock.Object, NullLogger<StaffLogic>.Instance);

            Assert.Throws<BusinessLayerException>(() => staffLogic.ReportParcelDelivery(trackingId));
        }

        [Fact]
        public void ReportParcelDelivery_IsOk()
        {
            string trackingId = "123456789";
            DA.Parcel p = new DA.Parcel() { TrackingId = trackingId };

            Mock<IParcelRepository> parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(p);
            parcelRepoMock.Setup(foo => foo.Update(It.IsAny<DA.Parcel>()));

            Mock<IHopRepository> hopRepoMock = new Mock<IHopRepository>();
            Mock<IWebhookRepository> hookMock = new Mock<IWebhookRepository>();
            List<DA.Webhook> webhooks = new List<DA.Webhook>
            {
                new DA.Webhook(),
                new DA.Webhook()
            };
            hookMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(webhooks);

            IStaffLogic staffLogic = new StaffLogic(mapper, parcelRepoMock.Object, hopRepoMock.Object, hookMock.Object, NullLogger<StaffLogic>.Instance);

            //if it doesn't throw, it's ok
            staffLogic.ReportParcelDelivery(trackingId);
        }

        [Fact]
        public void ReportParcelHop_GetByTrackingIdExpection()
        {
            string trackingId = "123456789";
            string code = "987654321";

            Mock<IParcelRepository> parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(foo => foo.GetByTrackingId(trackingId)).Throws(new ParcelNotFoundExpection());

            Mock<IHopRepository> hopRepoMock = new Mock<IHopRepository>();
            Mock<IWebhookRepository> hookMock = new Mock<IWebhookRepository>();

            IStaffLogic staffLogic = new StaffLogic(mapper, parcelRepoMock.Object, hopRepoMock.Object, hookMock.Object, NullLogger<StaffLogic>.Instance);

            Assert.Throws<BusinessLayerException>(() => staffLogic.ReportParcelHop(trackingId, code));
        }

        [Fact]
        public void ReportParcelHop_GetByCodeExpection()
        {
            string trackingId = "123456789";
            string code = "987654321";
            DA.Parcel p = new DA.Parcel() { TrackingId = trackingId };

            Mock<IParcelRepository> parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(p);

            Mock<IHopRepository> hopRepoMock = new Mock<IHopRepository>();
            hopRepoMock.Setup(foo => foo.GetByCode(code)).Throws(new HopNotFoundExpection());
            Mock<IWebhookRepository> hookMock = new Mock<IWebhookRepository>();

            IStaffLogic staffLogic = new StaffLogic(mapper, parcelRepoMock.Object, hopRepoMock.Object, hookMock.Object, NullLogger<StaffLogic>.Instance);

            Assert.Throws<BusinessLayerException>(() => staffLogic.ReportParcelHop(trackingId, code));
        }

        [Fact]
        public void ReportParcelHop_UpdateExpection()
        {
            string trackingId = "123456789";
            string code = "987654321";
            DA.Parcel p = new DA.Parcel() { TrackingId = trackingId };
            DA.Warehouse w = new DA.Warehouse() { Code = code };

            Mock<IParcelRepository> parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(p);
            parcelRepoMock.Setup(foo => foo.Update(It.IsAny<DA.Parcel>())).Throws(new ParcelNotFoundExpection());

            Mock<IHopRepository> hopRepoMock = new Mock<IHopRepository>();
            hopRepoMock.Setup(foo => foo.GetByCode(code)).Returns(w);
            Mock<IWebhookRepository> hookMock = new Mock<IWebhookRepository>();

            IStaffLogic staffLogic = new StaffLogic(mapper, parcelRepoMock.Object, hopRepoMock.Object, hookMock.Object, NullLogger<StaffLogic>.Instance);

            Assert.Throws<BusinessLayerException>(() => staffLogic.ReportParcelHop(trackingId, code));
        }

        [Fact]
        public void ReportParcelHop_IsOk()
        {
            string trackingId = "123456789";
            string code = "987654321";
            DA.Parcel p = new DA.Parcel() { TrackingId = trackingId };
            DA.Warehouse w = new DA.Warehouse() { Code = code };

            Mock<IParcelRepository> parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(p);
            parcelRepoMock.Setup(foo => foo.Update(It.IsAny<DA.Parcel>()));

            Mock<IHopRepository> hopRepoMock = new Mock<IHopRepository>();
            hopRepoMock.Setup(foo => foo.GetByCode(code)).Returns(w);
            Mock<IWebhookRepository> hookMock = new Mock<IWebhookRepository>();
            List<DA.Webhook> webhooks = new List<DA.Webhook>
            {
                new DA.Webhook(),
                new DA.Webhook()
            };
            hookMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(webhooks);

            IStaffLogic staffLogic = new StaffLogic(mapper, parcelRepoMock.Object, hopRepoMock.Object, hookMock.Object, NullLogger<StaffLogic>.Instance);

            //if it doesn't throw, it's ok
            staffLogic.ReportParcelHop(trackingId, code);
        }
    }
}
