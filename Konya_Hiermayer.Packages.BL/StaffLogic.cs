
using System.Globalization;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Data = Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;

using System.Threading;
using Konya_Hiermayer.Packages.BL.Entities.Validators;
using FluentValidation.Results;
using DAL = Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.BL.Extensions;
using Konya_Hiermayer.Packages.DAL.SQL;


namespace Konya_Hiermayer.Packages.BL
{
    public class StaffLogic : IStaffLogic
    {
        private readonly IMapper mapper;
        private readonly IParcelRepository parcelRepository;
        private readonly IHopRepository hopRepository;
        private readonly IWebhookRepository webhookRepository;
        private readonly ILogger<StaffLogic> logger;
        private readonly IGeoCodingAgent geoCodingAgent;   
        private readonly IWarehouseRepository wareHouseRepository;
        private readonly IRouteCalculator routeCalculator;

        
        public StaffLogic(IMapper mapper, IParcelRepository parcelRepository, IHopRepository hopRepository, IWebhookRepository webhookRepository, ILogger<StaffLogic> logger,
        IGeoCodingAgent geoCodingAgent, IWarehouseRepository wareHouseRepository, IRouteCalculator routeCalculator)
        {
            this.mapper = mapper;
            this.parcelRepository = parcelRepository;
            this.hopRepository = hopRepository;
            this.logger = logger;
            this.webhookRepository = webhookRepository;
            this.geoCodingAgent = geoCodingAgent;
            this.wareHouseRepository = wareHouseRepository;
            this.routeCalculator = routeCalculator;
        }

        public void ReportParcelDelivery(string parcelID)
        {
            Data.Parcel dataParcel;
            try
            {
                logger.LogDebug($"getting parcel {parcelID} from repo");
                dataParcel = parcelRepository.GetByTrackingId(parcelID);
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }

            Parcel businessParcel = this.mapper.Map<Parcel>(dataParcel);
            businessParcel.ReportDelivery();
            Data.Parcel mappedParcel = this.mapper.Map<Data.Parcel>(businessParcel);

            try
            {
                logger.LogInformation($"updating parcel {parcelID}");
                parcelRepository.Delivered(mappedParcel);

                List<Data.Webhook> dataWebhooks = webhookRepository.GetByTrackingId(parcelID);
                List<Webhook> webhooks = new List<Webhook>();
                dataWebhooks.ForEach(hook => webhooks.Add(this.mapper.Map<Webhook>(hook)));
                NotifyAllSubscribers(webhooks);
                DeleteAllSubscribers(webhooks);
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }
        }

        public void ReportParcelHop(string parcelID, string code)
        {
            Data.Parcel dataParcel;
            try
            {
                logger.LogDebug($"getting parcel {parcelID} from repo");
                dataParcel = parcelRepository.GetByTrackingId(parcelID);
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }

            Parcel businessParcel = this.mapper.Map<Parcel>(dataParcel);
            Data.Hop dataHop;
            try
            {
                logger.LogDebug($"getting hop {code} from repo");
                dataHop = hopRepository.GetByCode(code);
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);

            }
            Hop businessHop = this.mapper.Map<Hop>(dataHop);
            businessParcel.ReportHop(businessHop);

            string senderGeoString = businessParcel.Sender.ToGeoCodingString();
            string recipientGeoString = businessParcel.Recipient.ToGeoCodingString();
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

            logger.LogDebug($"calculating route betweend sender {senderHop.Code} and recipeint {recipientHop.Code}");
            List<HopArrival> route = routeCalculator.CalculateRoute(warehouse, senderHop.Code, recipientHop.Code, businessParcel.EntryDate);
            //List<HopArrival> route = routeCalculator.CalculateRoute(warehouse, senderHop.Code, recipientHop.Code);    

            bool checkIfOnRoute = false;

            foreach (HopArrival ha in route)
            {
                if(ha.Hop.Code == businessHop.Code)
                {
                    checkIfOnRoute = true;
                    break;
                }
            }

            if (!checkIfOnRoute)
            {
                throw new BusinessLayerException("Hop is not on TravelRoute");
            }

            try
            {
                logger.LogInformation($"updating parcel {parcelID}");
                parcelRepository.Update(this.mapper.Map<Data.Parcel>(businessParcel));

                List<Data.Webhook> dataWebhooks = webhookRepository.GetByTrackingId(parcelID);
                List<Webhook> webhooks = new List<Webhook>();
                dataWebhooks.ForEach(hook => webhooks.Add(this.mapper.Map<Webhook>(hook)));
                NotifyAllSubscribers(webhooks);
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }
        }

        private void NotifyAllSubscribers(List<Webhook> subs)
        {
            foreach (var sub in subs)
            {
                try
                {
                    logger.LogInformation($"notifying subscriber for parcel {sub.TrackingId} on \"{sub.Url}\"");
                    HttpClient client = new HttpClient();
                    //var json = JsonConvert.SerializeObject(person);
                    //var data = new StringContent(json, Encoding.UTF8, "application/json");
                    client.PostAsync(sub.Url, new StringContent(sub.TrackingId));
                }
                catch (Exception e)
                {
                    // Not throwing here cause Notification error should not crash the API
                    logger.LogError($"error notifiying for parcel {sub.TrackingId} on \"{sub.Url}\"");
                    logger.LogError(e.ToString());
                }
            }
        }

        private void DeleteAllSubscribers(List<Webhook> subs)
        {
            foreach (var sub in subs)
            {
                try
                {
                    webhookRepository.DeleteByWebhookId(sub.Id);
                }
                catch (Exception e)
                {
                    // Not throwing here cause error deleting hook should not crash the API
                    logger.LogError($"error deleting sub for parcel {sub.TrackingId} on \"{sub.Url}\" with id {sub.Id}");
                    logger.LogError(e.ToString());
                }
            }
        }
    }
}
