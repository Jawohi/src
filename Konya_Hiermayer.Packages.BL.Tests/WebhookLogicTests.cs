using System;
using AutoMapper;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.Mapping;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Xunit;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Data = Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.BL.Tests
{
    public class WebhookLogicTests : IDisposable
    {
        IMapper mapper;

        public WebhookLogicTests()
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
        public void GetAllSubscriptionsFromParcel_IsOk()
        {
            string trackingId = "ABCD12345";
            string url1 = "martö";
            string url2 = "michl";
            Mock<IWebhookRepository> webMock = new Mock<IWebhookRepository>();
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();
            List<Data.Webhook> webhooks = new List<Data.Webhook>
            {
                new Data.Webhook() { Id = 1, Url = url1, TrackingId = trackingId },
                new Data.Webhook() { Id = 2, Url = url2, TrackingId = trackingId }
            };
            webMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(webhooks);
                

            IWebhookLogic webhookLogic = new WebhookLogic(mapper, webMock.Object, parcelMock.Object, NullLogger<WebhookLogic>.Instance);

            List<Webhook> hooksInDB = webhookLogic.GetAllSubscriptionsFromParcel(trackingId);

            Assert.Equal(1, hooksInDB[0].Id);
            Assert.Equal(2, hooksInDB[1].Id);
            Assert.Equal(trackingId, hooksInDB[0].TrackingId);
            Assert.Equal(trackingId, hooksInDB[1].TrackingId);
            Assert.Equal(url1, hooksInDB[0].Url);
            Assert.Equal(url2, hooksInDB[1].Url);
        }

        [Fact]
        public void GetAllSubscriptionsFromParcel_NoWebhooksForTrackingID()
        {
            string trackingId = "ABCD12345";
            Mock<IWebhookRepository> webMock = new Mock<IWebhookRepository>();
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();

            webMock.Setup(foo => foo.GetByTrackingId(trackingId)).Throws(new WebhookNotFoundExpection());

            IWebhookLogic webhookLogic = new WebhookLogic(mapper, webMock.Object, parcelMock.Object, NullLogger<WebhookLogic>.Instance);              

            Assert.Throws<BusinessLayerException>(() => webhookLogic.GetAllSubscriptionsFromParcel(trackingId));
        }

        [Fact]
        public void SubscribeToParcel_IsOk()
        {
            string trackingId = "ABCD12345";
            string url = "url";
            Mock<IWebhookRepository> webMock = new Mock<IWebhookRepository>();
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();

            parcelMock.Setup(foo => foo.GetByTrackingId(trackingId)).Returns(new Data.Parcel());

            Data.Webhook webhook = new Data.Webhook() { CreatedAt = DateTime.Now, TrackingId = trackingId, Url = url };
            webMock.Setup(foo => foo.Create(webhook));

            IWebhookLogic webhookLogic = new WebhookLogic(mapper, webMock.Object, parcelMock.Object, NullLogger<WebhookLogic>.Instance);

            webhookLogic.SubscribeToParcel(trackingId, url);
        }

        [Fact]
        public void SubscribeToParcel_ParcelNotFound()
        {
            string trackingId = "ABCD12345";
            Mock<IWebhookRepository> webMock = new Mock<IWebhookRepository>();
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();

            parcelMock.Setup(foo => foo.GetByTrackingId(trackingId)).Throws(new ParcelNotFoundExpection());

            IWebhookLogic webhookLogic = new WebhookLogic(mapper, webMock.Object, parcelMock.Object, NullLogger<WebhookLogic>.Instance);

            Assert.Throws<BusinessLayerException>(() => webhookLogic.SubscribeToParcel(trackingId, "url"));
        }

        [Fact]
        public void UnsubscribeFromParcel_IsOk()
        {
            long id = 123;
            Mock<IWebhookRepository> webMock = new Mock<IWebhookRepository>();
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();

            webMock.Setup(foo => foo.DeleteByWebhookId(id));

            IWebhookLogic webhookLogic = new WebhookLogic(mapper, webMock.Object, parcelMock.Object, NullLogger<WebhookLogic>.Instance);

            webhookLogic.UnsubscribeFromParcel(id);
        }

        [Fact]
        public void UnsubscribeFromParcel_ParcelNotFound()
        {
            long id = 123;
            Mock<IWebhookRepository> webMock = new Mock<IWebhookRepository>();
            Mock<IParcelRepository> parcelMock = new Mock<IParcelRepository>();

            IWebhookLogic webhookLogic = new WebhookLogic(mapper, webMock.Object, parcelMock.Object, NullLogger<WebhookLogic>.Instance);

            webMock.Setup(foo => foo.DeleteByWebhookId(id)).Throws(new ParcelNotFoundExpection());

            Assert.Throws<BusinessLayerException>(() => webhookLogic.UnsubscribeFromParcel(id));
        }
    }
}

