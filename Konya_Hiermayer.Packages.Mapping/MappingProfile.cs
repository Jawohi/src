using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Net.Security;
using System.Security.Cryptography;
using AutoMapper;
using System;
using Service = Konya_Hiermayer.Packages.Services.Models;
using Business = Konya_Hiermayer.Packages.BL.Entities;
using Data = Konya_Hiermayer.Packages.DAL.Entities;
using System.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.IO;
using NetTopologySuite.Features;
using System.Text;
using System.Collections.Generic;

namespace Konya_Hiermayer.Packages.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // *********************
            // Service -> BL
            // *********************
            CreateMap<Service.Hop, Business.Hop>()
                .Include<Service.Warehouse, Business.Warehouse>()
                .Include<Service.Truck, Business.Truck>()
                .Include<Service.Transferwarehouse, Business.Transferwarehouse>();

            CreateMap<Business.Hop, Service.Hop>()
                .Include<Business.Warehouse, Service.Warehouse>()
                .Include<Business.Truck, Service.Truck>()
                .Include<Business.Transferwarehouse, Service.Transferwarehouse>();

            CreateMap<Service.Warehouse, Business.Warehouse>()
                .ReverseMap()
                .ForMember(dest => dest.HopType, opts => opts.MapFrom(src => src.GetType().Name));
            CreateMap<Service.Truck, Business.Truck>()
                .ReverseMap()
                .ForMember(dest => dest.HopType, opts => opts.MapFrom(src => src.GetType().Name));
            CreateMap<Service.Transferwarehouse, Business.Transferwarehouse>()
                .ReverseMap()
                .ForMember(dest => dest.HopType, opts => opts.MapFrom(src => src.GetType().Name));

            CreateMap<Service.HopArrival, Business.HopArrival>()
                .ForMember(dest => dest.Hop, opts => opts.MapFrom(src => new Business.Warehouse() { Code=src.Code })) //Checken
                .ReverseMap()
                .ForMember(dest => dest.Code, opts => opts.MapFrom(src => src.Hop.Code));

            CreateMap<Service.Recipient, Business.Recipient>().ReverseMap();
            CreateMap<Service.WarehouseNextHops, Business.WarehouseNextHops>().ReverseMap();
            CreateMap<Business.Parcel, Service.TrackingInformation>().ReverseMap();
            CreateMap<Business.Parcel, Service.Parcel>().ReverseMap();
            CreateMap<Business.Parcel, Service.NewParcelInfo>().ReverseMap();

            // *********************
            // BL -> DAL
            // *********************

            CreateMap<Business.Hop, Data.Hop>()
                  .Include<Business.Warehouse, Data.Warehouse>()
                  .Include<Business.Truck, Data.Truck>()
                  .ForMember(x => x.latitude,opt => opt.MapFrom(s => s.LocationCoordinates.Lat))
                  .ForMember(x => x.longitude,opt => opt.MapFrom(s => s.LocationCoordinates.Lon))
                  .Include<Business.Transferwarehouse, Data.Transferwarehouse>();

            CreateMap<Data.Hop, Business.Hop>()
                  .Include<Data.Warehouse, Business.Warehouse>()
                  .Include<Data.Truck, Business.Truck>()
                  .Include<Data.Transferwarehouse, Business.Transferwarehouse>();

            CreateMap<Business.Warehouse, Data.Warehouse>().ReverseMap();

            CreateMap<Service.GeoCoordinate, Business.GeoCoordinate>().ReverseMap(); 


            CreateMap<Business.Truck, Data.Truck>()
            .ForMember(x => x.RegionGeoJson, opt => opt.ConvertUsing(new GeoConverter(), src => src.RegionGeoJson)).ReverseMap();
            // .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom(src => GeoToGeoJson(src.RegionGeoJson)));
            CreateMap<Business.Transferwarehouse, Data.Transferwarehouse>()
            .ForMember(x => x.RegionGeoJson, opt => opt.ConvertUsing(new GeoConverter(), src => src.RegionGeoJson)).ReverseMap();
                // .ForMember(dest => dest.RegionGeo, opt => opt.MapFrom(src => GeoJsonToGeo(src.RegionGeoJson)))
                // .ReverseMap()
                // .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom(src => GeoToGeoJson(src.RegionGeo)));

            CreateMap<Business.WarehouseNextHops, Data.WarehouseNextHops>()
                .ForMember(dest => dest.SecondHop, opt => opt.MapFrom(src => src.Hop))
                .ForMember(dest => dest.SecondHopId, opt => opt.MapFrom(src => src.Hop.Code))
                .ForMember(dest => dest.FirstHop, opt => opt.Ignore())
                .ForMember(dest => dest.FirstHopId, opt => opt.Ignore());

            CreateMap<Data.WarehouseNextHops, Business.WarehouseNextHops>()
                .ForMember(dest => dest.Hop, opt => opt.MapFrom(src => src.SecondHop));

            CreateMap<Business.HopArrival, Data.HopArrival>()
                .ForMember(dest => dest.Hop, opt => opt.MapFrom(src => src.Hop))
                .ForMember(dest => dest.HopId, opt => opt.MapFrom(src => src.Hop.Code))
                .ForMember(dest => dest.Parcel, opt => opt.Ignore())
                .ForMember(dest => dest.ParcelId, opt => opt.Ignore())
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Data.HopArrival, Business.HopArrival>()
                .ForMember(dest => dest.Hop, opt => opt.MapFrom(src => src.Hop))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForAllOtherMembers(opt => opt.Ignore());




            CreateMap<Business.Parcel, Data.Parcel>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Hops, opt => opt.MapFrom(src => src.VisitedHops))
                .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(src => src.EntryDate));
                

            CreateMap<Data.Parcel, Business.Parcel>();

            CreateMap<Business.Recipient, Data.Recipient>().ReverseMap();

            CreateMap<Business.Webhook, Service.WebhookResponse>().ReverseMap();
            CreateMap<Business.Webhook, Data.Webhook>().ReverseMap();

        }

        private void SetVisitedFlag(List<Business.HopArrival> hops)
        {

        }
        
    }
    public class GeoConverter : IValueConverter<string, Geometry>
        {
            public Geometry Convert(string sourceMember, ResolutionContext context)
            {
                var reader = new GeoJsonReader();
                var feature = reader.Read<Feature>(sourceMember);
                Geometry geo = feature.Geometry.Normalized().Reverse();
                return geo;
            }
        }
}