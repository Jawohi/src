using System.Net;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.SQL;
using Konya_Hiermayer.Packages.BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL
{
    public class RouteCalculator : IRouteCalculator
    {
        public List<HopArrival> CalculateRoute(Warehouse hierarchy, string codeSender, string codeRecipient, DateTime? entryDate)
        //public List<HopArrival> CalculateRoute(Warehouse hierarchy, string codeSender, string codeRecipient)
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
            //return CalculateTravelTime(route);
        }


        private List<HopArrival> CalculateTravelTime(List<Hop> route, DateTime? travelDates)
        //private List<HopArrival> CalculateTravelTime(List<Hop> route)
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
        
            for (int i = pathSender.Count - 1; i >= 0; i--)
            {
                if (pathSender[i].Code == pathRecipient[i].Code)
                {
                    return i;
                }
            }
            throw new NoPathFoundException(" No Matching Path Found");
        }
    }

    public class PathFinder
    {
        string code;

        public List<Hop> CalculateRoute(Warehouse hierarchy, string code)
        {
            this.code = code;
            List<Hop> route = CheckNextHop(hierarchy);
            route.Insert(0, hierarchy);
            return route;
        }

        private List<Hop> CheckNextHop(Warehouse hierarchy)
        {
            List<Hop> route = new List<Hop>();
            foreach (WarehouseNextHops whnh in hierarchy.NextHops)
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