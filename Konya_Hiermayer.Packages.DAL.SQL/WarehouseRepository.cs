using System.Net.Http.Headers;
using System.Xml;
using System;
using System.Linq;
using System.Collections.Generic;
using Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Konya_Hiermayer.Packages.DAL.SQL
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly DatabaseContext context;
        private readonly ILogger<WarehouseRepository> logger;

        public WarehouseRepository(DatabaseContext context, ILogger<WarehouseRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }


        public void Create(Warehouse warehouse)
        {
            try
            {
                Delete();
                context.Warehouses.Add(warehouse);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Duplicate {warehouse.Code} Warehouse in DB");
                throw new DuplicateWarehouseExpection();
            }
        }

        public void Update(Warehouse warehouse)
        {
            try
            {
                context.Warehouses.Update(warehouse);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Warehouse {warehouse.Code} not found in DB");
                throw new WarehouseNotFoundExpection();
            }
        }

        public void Delete()
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public Warehouse Read()
        {
            try
            {
                Warehouse a = context.Hops.OfType<Warehouse>().Include(w => w.NextHops).ThenInclude(nh => nh.SecondHop).AsEnumerable()
                    .Where(h => h.Description.Contains("Root Warehouse")).ToList().FirstOrDefault();
                return a;

            }
            catch (Exception e)
            {
                logger.LogError(e, $"Warehouse for complete hierarchy not found");
                throw new WarehouseNotFoundExpection();
            }
        }

        private List<Hop> CheckNextHop(Warehouse hirarchy, string code)
        {
            List<Hop> route = new List<Hop>();
            foreach (WarehouseNextHops whnh in hirarchy.NextHops)
            {
                if (whnh.SecondHop.Description.Contains("Truck") || whnh.SecondHop.Description.Contains("Transferwarehouse"))
                {
                    if (whnh.SecondHop.Code == code)
                    {
                        route.Add(whnh.SecondHop);
                        return route;
                    }
                    continue;
                }

                List<Hop> subRoute = CheckNextHop((Warehouse)whnh.SecondHop, code);
                if (subRoute.Count == 0)
                    continue;
                route.AddRange(subRoute);
                return route;
            }
            return route;
        }

        public Warehouse GetByCode(string code)
        {
            try
            {
                Warehouse w = new Warehouse();
                    //returns a Truck Disguised as a Warehouse
                Warehouse hierachy = Read();
                List<Hop>  CanBeTruck = CheckNextHop(hierachy, code);
                if (CanBeTruck.Count != 0)
                {
                    w.Code = code;
                    w.Description = CanBeTruck[0].Description;
                    w.NextHops = null;
                    w.ProcessingDelayMins = CanBeTruck[0].ProcessingDelayMins;
                    w.RegionGeoJson = null;
                }
                if (w.Description == null)
                {
                    w = context.Warehouses.Single(h => h.Code == code);
                    w.NextHops = null;
                }
                return w;
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Warehouse {code} not found in DB");
                throw new WarehouseNotFoundExpection();
            }




        }
    }
}
