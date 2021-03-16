using System.Net;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL
{
    public class RouteCalculator : IRouteCalculator
    {
        public List<HopArrival> CalculateRoute(Warehouse hierarchy, string codeSender, string codeRecipient, DateTime? entryDate)
        {
            List<Hop> pathSender = new PathFinder().CalculateRoute(hierarchy, codeSender);
            List<Hop> pathRecipient = new PathFinder().CalculateRoute(hierarchy, codeRecipient);

            int crossingIndex = FindCrossingWarehouseIndex(pathSender, pathRecipient);
            List<Hop> route = new List<Hop>();

            for (int i = pathSender.Count - 1; i > crossingIndex; i--)
            {
                route.Add(pathSender[i]);
            }
            for (int i = crossingIndex; i < pathRecipient.Count; i++)
            {
                route.Add(pathRecipient[i]);
            }

            return CalculateTravelTime(route, entryDate);
        }


        private List<HopArrival> CalculateTravelTime(List<Hop> route, DateTime? travelDates)
        {
            List<HopArrival> hopArrivals = new List<HopArrival>();
            DateTime travelDate = (DateTime)travelDates;
            for (int i = 0; i < route.Count; i++)
            {
                hopArrivals.Add(new HopArrival(route[i], travelDate));
                travelDate = travelDate.AddMinutes(route[i].ProcessingDelayMins.Value);
                if (i < route.Count - 1)
                {
                    if (route[i + 1] is Warehouse)
                    {
                        foreach (WarehouseNextHops wnh in ((Warehouse)route[i + 1]).NextHops)
                        {
                            if (wnh.Hop.Code == route[i].Code)
                            {
                                travelDate = travelDate.AddMinutes(wnh.TraveltimeMins.Value);
                                break;
                            }
                        }
                    }
                }
            }
            return hopArrivals;
        }

        private int FindCrossingWarehouseIndex(List<Hop> pathSender, List<Hop> pathRecipient)
        {
            //iterate List backwards
            for (int i = pathSender.Count - 1; i >= 0; i--)
            {
                if (pathSender[i].Code == pathRecipient[i].Code)
                {
                    return i;
                }
            }
            throw new Exception(); //TODO: Austauschen keinen gemeinsamen Pfad gefunden
        }
    }

    public class PathFinder
    {
        string code;

        public List<Hop> CalculateRoute(Warehouse hirarchy, string code)
        {
            this.code = code;
            List<Hop> route = CheckNextHop(hirarchy);
            route.Insert(0, hirarchy);
            return route;
        }

        private List<Hop> CheckNextHop(Warehouse hirarchy)
        {
            List<Hop> route = new List<Hop>();
            foreach (WarehouseNextHops whnh in hirarchy.NextHops)
            {
                if (whnh.Hop.Description.Contains("Truck") || whnh.Hop.Description.Contains("Transferwarehouse"))
                {
                    if (whnh.Hop.Code == code)
                    {
                        route.Add(whnh.Hop);
                        return route;
                    }
                    continue;
                }

                List<Hop> subRoute = CheckNextHop((Warehouse)whnh.Hop);
                if (subRoute.Count == 0)
                    continue;

                route.Add(whnh.Hop);
                route.AddRange(subRoute);
                return route;
            }
            return route;
        }
    }
}