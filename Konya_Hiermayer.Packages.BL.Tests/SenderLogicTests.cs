using System;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Konya_Hiermayer.Packages.Mapping;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;
using Xunit;

namespace Konya_Hiermayer.Packages.BL.Tests
{
    public class SenderLogicTests : IDisposable
    {
        IMapper mapper;

        public SenderLogicTests()
        {
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            mapper = new Mapper(configuration);
        }

        public void Dispose()
        {

        }

        // kind of useless test since there is no logic in there
        // can hardly be tested since a random string is generated in the method
        // just for code coverage
        [Fact]
        public void SubmitNewParcel_IsOk()
        {
            string trackingID = "ABCD12345";
            Parcel p = new Parcel() { TrackingId = trackingID };

            Mock<ILogisticsPartnerLogic> partnerMock = new Mock<ILogisticsPartnerLogic>();
            partnerMock.Setup(foo => foo.TransferParcel(It.IsAny<String>(), p)).Returns(trackingID);

            ISenderLogic senderLogic = new SenderLogic(partnerMock.Object, NullLogger<SenderLogic>.Instance);

            string resultID = senderLogic.SubmitNewParcel(p);

            Assert.Equal(trackingID, resultID);
        }
    }
}
