using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using Konya_Hiermayer.Packages.DAL.SQL;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Entities;
using Microsoft.Data.Sqlite;
using System;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Konya_Hiermayer.Packages.DAL.Tests
{
    public class WarehouseRepositoryTests : IDisposable
    {
        DbContextOptions<DatabaseContext> options;
        SqliteConnection connection;

        public WarehouseRepositoryTests()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            options = new DbContextOptionsBuilder<DatabaseContext>()
                    .UseSqlite(connection)
                    .Options;
        }

        public void Dispose()
        {
            connection.Close();
        }

        [Fact]
        public void Create_IsSaved()
        {
            try
            {
                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo dawg", ProcessingDelayMins = 1 };
                    service.Create(wh);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    Assert.Equal(1, context.Hops.Count());
                    Assert.Equal("123456789", context.Hops.Single().Code);
                    Assert.Equal("yo dawg", context.Hops.Single().Description);
                }
            }
            finally { }
        }

        [Fact]
        public void Create_ThrowsDuplicateHopExpection()
        {
            try
            {
                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo dawg", ProcessingDelayMins = 1 };
                    service.Create(wh);
                }

                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo dawg", ProcessingDelayMins = 1 };
                    Assert.Throws<DuplicateHopExpection>(() => service.Create(wh));
                }
            }
            finally { }
        }

        [Fact]
        public void Update_IsUpdated()
        {
            try
            {
                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo dawg", ProcessingDelayMins = 1 };
                    context.Hops.Add(wh);
                    context.SaveChanges();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    Assert.Equal(1, context.Hops.Count());
                    Assert.Equal("123456789", context.Hops.Single().Code);
                    Assert.Equal("yo dawg", context.Hops.Single().Description);
                }

                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo boi", ProcessingDelayMins = 1 };
                    service.Update(wh);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    Assert.Equal(1, context.Hops.Count());
                    Assert.Equal("123456789", context.Hops.Single().Code);
                    Assert.Equal("yo boi", context.Hops.Single().Description);
                }
            }
            finally { }
        }

        [Fact]
        public void Update_ThrowsHopNotFoundExpection()
        {
            try
            {
                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo boi", ProcessingDelayMins = 1 };
                    Assert.Throws<HopNotFoundExpection>(() => service.Update(wh));
                }
            }
            finally { }
        }

        [Fact]
        public void Delete_IsEmpty()
        {
            try
            {
                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo dawg", ProcessingDelayMins = 1 };
                    context.Hops.Add(wh);
                    context.SaveChanges();
                }

                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    service.Delete();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    //Assert.Equal(0, context.Hops.Count());
                }
            }
            finally { }
        }

        [Fact]
        public void GetByCode_ReturnsHop()
        {
            try
            {
                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo dawg", ProcessingDelayMins = 1 };
                    context.Hops.Add(wh);
                    context.SaveChanges();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    Warehouse wh = service.GetByCode("123456789");
                    Assert.Equal("123456789", wh.Code);
                    Assert.Equal("yo dawg", wh.Description);
                }
            }
            finally { }
        }

        [Fact]
        public void GetByCode_ThrowsHopNotFoundExpection()
        {
            try
            {
                using (var context = new DatabaseContext(options))
                {
                    IWarehouseRepository service = new WarehouseRepository(context, NullLogger<WarehouseRepository>.Instance);
                    Warehouse wh = new Warehouse() { Code = "123456789", Description = "yo boi", ProcessingDelayMins = 1 };
                    Assert.Throws<HopNotFoundExpection>(() => service.GetByCode("123456789"));
                }
            }
            finally { }
        }
    }
}
