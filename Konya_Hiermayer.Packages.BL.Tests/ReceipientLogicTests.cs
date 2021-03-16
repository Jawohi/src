using System;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using DA = Konya_Hiermayer.Packages.DAL.Entities;
using Xunit;
using Moq;
using AutoMapper;
using Konya_Hiermayer.Packages.Mapping;
using Microsoft.Extensions.Logging.Abstractions;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;

namespace Konya_Hiermayer.Packages.BL.Tests
{
    public class RecipientLogicTests : IDisposable
    {
        IMapper mapper;

        public RecipientLogicTests()
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
        public void TrackParcel_ReturnParcel()
        {
            string id = "123456789";

            Mock<IParcelRepository> mock = new Mock<IParcelRepository>();
            DA.Parcel wh = new DA.Parcel() { TrackingId = id, State=DA.Parcel.StateEnum.DeliveredEnum };
            mock.Setup(foo => foo.GetByTrackingId(id)).Returns(wh);

            IRecipientLogic receipientLogic = new RecipientLogic(mapper, mock.Object, NullLogger<RecipientLogic>.Instance );

            Parcel p = receipientLogic.TrackParcel(id);

            Assert.NotNull(p);
            Assert.Equal(p.TrackingId, id);
            Assert.Equal(Parcel.StateEnum.DeliveredEnum, p.State);
        }


        [Fact]
        public void TrackParcel_ParcelNotFound()
        {
            string id = "123456789";

            Mock<IParcelRepository> mock = new Mock<IParcelRepository>();
            DA.Parcel wh = new DA.Parcel() { TrackingId = id, State = DA.Parcel.StateEnum.DeliveredEnum };

            mock.Setup(foo => foo.GetByTrackingId(id)).Throws(new ParcelNotFoundExpection());

            IRecipientLogic receipientLogic = new RecipientLogic(mapper, mock.Object, NullLogger<RecipientLogic>.Instance);

            Assert.Throws<BusinessLayerException>(() => receipientLogic.TrackParcel(id));
        }
    }
}
