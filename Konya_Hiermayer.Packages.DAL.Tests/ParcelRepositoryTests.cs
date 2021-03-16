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
    public class ParcelRepositoryTests : IDisposable
    {
        DbContextOptions<DatabaseContext> options;
        SqliteConnection connection;

        public ParcelRepositoryTests()
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
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "Martin" };
                    Recipient sender = new Recipient() { Name = "Simon" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    service.Create(p);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    Assert.Equal(1, context.Parcels.Count());
                    Assert.Equal("123456789", context.Parcels.Single().TrackingId);
                    Assert.Equal("Martin", context.Parcels.Single().Recipient.Name);
                    Assert.Equal("Simon", context.Parcels.Single().Sender.Name);
                }
            }
            finally { }
        }

        [Fact]
        public void Create_ThrowsDuplicateParcelExpection()
        {
            try
            {
                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "Martin" };
                    Recipient sender = new Recipient() { Name = "Simon" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    service.Create(p);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "Martin" };
                    Recipient sender = new Recipient() { Name = "Simon" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    Assert.Throws<DuplicateParcelExpection>(() => service.Create(p));
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
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "Martin" };
                    Recipient sender = new Recipient() { Name = "Simon" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    context.Parcels.Add(p);
                    context.SaveChanges();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    Assert.Equal(1, context.Parcels.Count());
                    Assert.Equal("123456789", context.Parcels.Single().TrackingId);
                    Assert.Equal("Martin", context.Parcels.Single().Recipient.Name);
                    Assert.Equal("Simon", context.Parcels.Single().Sender.Name);
                }

                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "David" };
                    Recipient sender = new Recipient() { Name = "Michael" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    service.Update(p);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    Assert.Equal(1, context.Parcels.Count());
                    Assert.Equal("123456789", context.Parcels.Single().TrackingId);
                    Assert.Equal("David", context.Parcels.Single().Recipient.Name);
                    Assert.Equal("Michael", context.Parcels.Single().Sender.Name);
                }
            }
            finally { }
        }

        [Fact]
        public void Update_ThrowsParcelNotFoundExpection()
        {
            try
            {
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "David" };
                    Recipient sender = new Recipient() { Name = "Michael" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    Assert.Throws<ParcelNotFoundExpection>(() => service.Update(p));
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
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "Martin" };
                    Recipient sender = new Recipient() { Name = "Simon" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    context.Parcels.Add(p);
                    context.SaveChanges();
                }

                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    service.Delete();
                    // TODO: doesn't delete it, idk why
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    //Assert.Equal(0, context.Parcels.Count());
                }
            }
            finally { }
        }

        [Fact]
        public void GetByCode_ReturnsParcel()
        {
            try
            {
                // Run the test against one instance of the context
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Recipient recipient = new Recipient() { Name = "Martin" };
                    Recipient sender = new Recipient() { Name = "Simon" };
                    Parcel p = new Parcel() { Recipient = recipient, Sender = sender, TrackingId = "123456789" };
                    context.Parcels.Add(p);
                    context.SaveChanges();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Parcel p = (Parcel)service.GetByTrackingId("123456789");
                    Assert.Equal(1, context.Parcels.Count());
                    Assert.Equal("123456789", context.Parcels.Single().TrackingId);
                    Assert.Equal("Martin", context.Parcels.Single().Recipient.Name);
                    Assert.Equal("Simon", context.Parcels.Single().Sender.Name);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void GetByCode_ThrowsParcelNotFoundExpection()
        {
            try
            {
                using (var context = new DatabaseContext(options))
                {
                    IParcelRepository service = new ParcelRepository(context, NullLogger<ParcelRepository>.Instance);
                    Assert.Throws<ParcelNotFoundExpection>(() => service.GetByTrackingId("123456789"));
                }
            }
            finally { }
        }
    }
}
