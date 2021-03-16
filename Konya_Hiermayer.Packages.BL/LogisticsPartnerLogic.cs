
using System.Threading;
using System.Data.Common;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.BL.Entities;
using System;
using Konya_Hiermayer.Packages.BL.Entities.Validators;
using FluentValidation.Results;
using DAL = Konya_Hiermayer.Packages.DAL.Entities;
using AutoMapper;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;
using Konya_Hiermayer.Packages.BL.Extensions;
using Data = Konya_Hiermayer.Packages.DAL.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Konya_Hiermayer.Packages.DAL.SQL;

namespace Konya_Hiermayer.Packages.BL
{
    public class LogisticsPartnerLogic : ILogisticsPartnerLogic
    {
        private readonly IMapper mapper;
        private readonly IParcelRepository parcelRepository;
        private readonly IGeoCodingAgent geoCodingAgent;
        private readonly IHopRepository hopRepository;
        private readonly IWarehouseRepository wareHouseRepository;
        private readonly IRouteCalculator routeCalculator;
        private readonly ILogger<LogisticsPartnerLogic> logger;

        public LogisticsPartnerLogic(IMapper mapper, IParcelRepository parcelRepository, IWarehouseRepository wareHouseRepository, IHopRepository hopRepository, IGeoCodingAgent geoCodingAgent, IRouteCalculator routeCalculator, ILogger<LogisticsPartnerLogic> logger)
        {
            this.mapper = mapper;
            this.parcelRepository = parcelRepository;
            this.geoCodingAgent = geoCodingAgent;
            this.hopRepository = hopRepository;
            this.wareHouseRepository = wareHouseRepository;
            this.routeCalculator = routeCalculator;
            this.logger = logger;
        }

        public string TransferParcel(string parcelID, Parcel parcel)
        {
            string senderGeoString = parcel.Sender.ToGeoCodingString();
            string recipientGeoString = parcel.Recipient.ToGeoCodingString();
            logger.LogDebug($"converting sender geoCodingString '{senderGeoString}' to Location");
            logger.LogDebug($"converting recepient geoCodingString '{recipientGeoString}' to Location");

            Geocoding.Location senderAddress = geoCodingAgent.EncodeAddress(senderGeoString);
            Geocoding.Location recipientAddress = geoCodingAgent.EncodeAddress(recipientGeoString);

            Data.Hop dataSenderHop;
            Data.Hop dataRecipientHop;
            try 
            {
                dataSenderHop = hopRepository.GetByCoordinates(senderAddress.Latitude,senderAddress.Longitude);
                dataRecipientHop = hopRepository.GetByCoordinates(recipientAddress.Latitude,recipientAddress.Longitude);
            }
            catch(DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }

            if (dataSenderHop == null || dataRecipientHop == null)
            {
                string errorMessage = "Error";

                NoHopException e = new NoHopException(errorMessage);
                logger.LogError(e, $"No Hop found");
                throw e;
            }

            Data.Warehouse dataWarehouse;
            try
            {
                logger.LogDebug("load full warehouse hierarchy");
                dataWarehouse = wareHouseRepository.Read();
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }

            Hop senderHop = this.mapper.Map<Hop>(dataSenderHop);
            Hop recipientHop = this.mapper.Map<Hop>(dataRecipientHop);
            Warehouse warehouse = this.mapper.Map<Warehouse>(dataWarehouse);

            logger.LogDebug($"Submiting parcel {parcelID}");
            parcel.Submit(parcelID);
            
            logger.LogDebug($"calculating route betweend sender {senderHop.Code} and recipeint {recipientHop.Code}");
            List<HopArrival> route = routeCalculator.CalculateRoute(warehouse, senderHop.Code, recipientHop.Code, parcel.EntryDate);

          
            

            logger.LogDebug($"validating parcel ({parcelID})");
            ParcelValidator validator = new ParcelValidator();
            ValidationResult result = validator.Validate(parcel);

            if (result.IsValid)
            {
                logger.LogDebug($"Parcel ({parcelID}) was valid");
                Data.Parcel dataParcel = this.mapper.Map<Data.Parcel>(parcel);
                
                
                logger.LogInformation("Creating Parcel DB entry");
                try { 
                    parcelRepository.Create(dataParcel);
                    return parcelID;
                }
                catch(DataAccessLayerException e)
                {
                    throw new BusinessLayerException("DAL Exception", e);
                }
            }

            else
            {
                InvalidParcelException e = new InvalidParcelException(string.Join("", result.Errors));
                logger.LogError(e, $"Parcel ({parcelID}) was invalid");
                throw e;
            }
        }
    }
}
