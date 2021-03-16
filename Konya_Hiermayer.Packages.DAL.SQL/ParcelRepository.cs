using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Konya_Hiermayer.Packages.DAL.Interfaces;

namespace Konya_Hiermayer.Packages.DAL.SQL
{
    public class ParcelRepository : IParcelRepository
    {

        private readonly DatabaseContext context;
        private readonly ILogger<ParcelRepository> logger;

        public ParcelRepository(DatabaseContext context, ILogger<ParcelRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void Create(Parcel parcel)
        {
            try 
            { 
                parcel.State = Parcel.StateEnum.PickupEnum;
                context.Parcels.Add(parcel);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Duplicate Parcel {parcel.TrackingId} in DB");
                throw new DuplicateParcelExpection();
            }
        }

        public void Delivered(Parcel parcel)
        {
            try
            {
                var deliveredParcel = new Parcel();
                deliveredParcel = context.Parcels.SingleOrDefault(p => p.TrackingId == parcel.TrackingId);
                deliveredParcel.State = Parcel.StateEnum.DeliveredEnum;
                // pchange enum somehow
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Parcel {parcel.TrackingId} not found in DB");
                throw new ParcelNotFoundExpection();
            }
        }

        public void Update(Parcel parcel)
        {
            try
            {
                var changedParcel = new Parcel();
                changedParcel = context.Parcels.SingleOrDefault(p => p.TrackingId == parcel.TrackingId);
                changedParcel.State = parcel.State;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Parcel {parcel.TrackingId} not found in DB");
                throw new ParcelNotFoundExpection();
            }
        }

        public void Delete()
        {
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
        }

        public Parcel GetByTrackingId(string trackingId)
        {
           
            try 
            { 
                return context.Parcels.Include(x => x.Hops).ThenInclude(x => x.Hop).Single(p => p.TrackingId == trackingId);

            }
            catch (Exception e)
            {
                logger.LogError(e, $"Parcel {trackingId} not found in DB");
                throw new ParcelNotFoundExpection();
            }
        }
    }
}
