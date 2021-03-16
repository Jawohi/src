using System;
using AutoMapper;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.Mapping;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using DA = Konya_Hiermayer.Packages.DAL.Entities;
using Xunit;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;

namespace Konya_Hiermayer.Packages.BL.Tests
{
    public class WarehouseManagementLogicTests : IDisposable
    {
        IMapper mapper;

        public WarehouseManagementLogicTests()
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
        public void ExportWarehouse_IsOk()
        {
            string code = "123456789";
            Mock<IWarehouseRepository> mock = new Mock<IWarehouseRepository>();
            DA.Warehouse wh = new DA.Warehouse() { Code = code };
            mock.Setup(foo => foo.Read()).Returns(wh);
            IWarehouseManagementLogic warehouseManagementLogic = new WarehouseManagementLogic(mapper, mock.Object, NullLogger<WarehouseManagementLogic>.Instance);

            Warehouse w = warehouseManagementLogic.ExportWarehouse();

            Assert.NotNull(w);
            Assert.Equal(code, w.Code);
        }

        [Fact]
        public void ExportWarehouse_ReadException()
        {
            Mock<IWarehouseRepository> mock = new Mock<IWarehouseRepository>();
            mock.Setup(foo => foo.Read()).Throws(new WarehouseNotFoundExpection());

            IWarehouseManagementLogic warehouseManagementLogic = new WarehouseManagementLogic(mapper, mock.Object, NullLogger<WarehouseManagementLogic>.Instance);

            Assert.Throws<BusinessLayerException>(() => warehouseManagementLogic.ExportWarehouse());
        }

        [Fact]
        public void ImportWarehouse_IsOk()
        {
            Mock<IWarehouseRepository> mock = new Mock<IWarehouseRepository>();

            WarehouseNextHops nexthop1 = new WarehouseNextHops() { Hop = new Warehouse(), TraveltimeMins = 1 };
            WarehouseNextHops nexthop2 = new WarehouseNextHops() { Hop = new Warehouse(), TraveltimeMins = 2 };
            IEnumerable<WarehouseNextHops> nextHops = new List<WarehouseNextHops>() { nexthop1, nexthop2 };
            Warehouse wh = new Warehouse() { Code = "123456789", Description = "a valid one", ProcessingDelayMins = 1, NextHops = nextHops };

            IWarehouseManagementLogic warehouseManagementLogic = new WarehouseManagementLogic(mapper, mock.Object, NullLogger<WarehouseManagementLogic>.Instance);

            warehouseManagementLogic.ImportWarehouse(wh);

            /// if it doesn't throw we good
        }

        [Theory]
        [InlineData("123456789", "invalid one @")]
        [InlineData("12356", "valid one @")]
        [InlineData("123", "invalid one @")]
        [InlineData("", "valid one")]
        public void ImportWarehouse_InvalidWarehouse(string code, string description)
        {
            Mock<IWarehouseRepository> mock = new Mock<IWarehouseRepository>();

            WarehouseNextHops nexthop1 = new WarehouseNextHops() { Hop = new Warehouse(), TraveltimeMins = 1 };
            WarehouseNextHops nexthop2 = new WarehouseNextHops() { Hop = new Warehouse(), TraveltimeMins = 2 };
            IEnumerable<WarehouseNextHops> nextHops = new List<WarehouseNextHops>() { nexthop1, nexthop2 };
            Warehouse wh = new Warehouse() { Code = code, Description = description, ProcessingDelayMins = 1, NextHops = nextHops };

            IWarehouseManagementLogic warehouseManagementLogic = new WarehouseManagementLogic(mapper, mock.Object, NullLogger<WarehouseManagementLogic>.Instance);

            Assert.Throws<InvalidWarehouseException>(() => warehouseManagementLogic.ImportWarehouse(wh));
        }
    }
}

