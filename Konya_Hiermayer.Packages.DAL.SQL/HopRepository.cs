using System.Diagnostics.Contracts;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Microsoft.Extensions.Logging;
using GeoAPI.Geometries;

namespace Konya_Hiermayer.Packages.DAL.SQL
{
    public class HopRepository : IHopRepository
    {
        private readonly DatabaseContext context;
        private readonly ILogger<HopRepository> logger;

        public HopRepository(DatabaseContext context, ILogger<HopRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void Create(Hop hop)
        {
            try { 
                context.Hops.Add(hop);
                context.SaveChanges();
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Duplicate Hop {hop.Code} in DB");
                throw new DuplicateHopExpection();
            }
        }

        public void Update(Hop hop)
        {
            try
            {
                context.Hops.Update(hop);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Hop {hop.Code} not found in DB");
                throw new HopNotFoundExpection();
            }
        }

        public void Delete()
        {
            // other data is useles w/o Warehouses
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
        }

        public Hop Read()
        {
            try 
            { 
                // TODO: save code of root warehouse and save it in DB!!! (and use it here obvsly)
                return context.Hops.FirstOrDefault();
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Hop not found in DB");
                throw new HopNotFoundExpection();
            }
        }

        public Hop GetByCode(string code)
        {
            try 
            { 
                return context.Hops.Single(h => h.Code == code);
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Hop {code} not found in DB");
                throw new HopNotFoundExpection();
            }
        }

        public Hop GetByCoordinates(IPoint point)
        {
           
            NetTopologySuite.Geometries.Point tmpPoint = new NetTopologySuite.Geometries.Point(point.X, point.Y);
            Hop hop = context.Hops.OfType<Truck>().FirstOrDefault(c => c.RegionGeoJson.Contains(tmpPoint));
            if (hop == null)
                hop = context.Hops.OfType<Transferwarehouse>().FirstOrDefault(c => c.RegionGeoJson.Contains(tmpPoint));

            return hop;
        }
        
        public Hop GetByCoordinates(double Y, double X)
        {
            NetTopologySuite.Geometries.Point tmpPoint = new NetTopologySuite.Geometries.Point(X, Y) {SRID = 4326};
            Hop hop = context.Hops.OfType<Truck>().FirstOrDefault(c => c.RegionGeoJson.Contains(tmpPoint));
            if (hop == null)
            {
                hop = context.Hops.OfType<Transferwarehouse>().FirstOrDefault(c => c.RegionGeoJson.Contains(tmpPoint));
                if (hop == null)
                {
                    hop = context.Hops.OfType<Warehouse>().FirstOrDefault(c => c.RegionGeoJson.Contains(tmpPoint));
                }
            }
            if(hop == null)
            {
                tmpPoint = new NetTopologySuite.Geometries.Point(Y, X);
                hop = context.Hops.OfType<Truck>().FirstOrDefault(c => c.RegionGeoJson.Contains(tmpPoint));
                if (hop == null)
                {
                    hop = context.Hops.OfType<Transferwarehouse>().FirstOrDefault(c => c.RegionGeoJson.Contains(tmpPoint));
                }
            }
            return hop;
        }
    }
}
