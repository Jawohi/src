
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Konya_Hiermayer.Packages.BL.Entities.Validators;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Data = Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;
using Konya_Hiermayer.Packages.BL.Extensions;
using Konya_Hiermayer.Packages.DAL.SQL;
using System.Net.Http;

namespace Konya_Hiermayer.Packages.BL
{
    public class RecipientLogic : IRecipientLogic
    {
        private readonly IMapper mapper;
        private readonly IParcelRepository parcelRepository;
        private readonly ILogger<RecipientLogic> logger;
        private readonly IGeoCodingAgent geoCodingAgent; 
        private readonly IRouteCalculator routeCalculator;
        private readonly IHopRepository hopRepository;
        private readonly IWarehouseRepository wareHouseRepository;
        private readonly IWebhookRepository webhookRepository;

        public RecipientLogic(IMapper mapper, IParcelRepository parcelRepository, ILogger<RecipientLogic> logger, IGeoCodingAgent geoCodingAgent, IRouteCalculator routeCalculator
        ,IWarehouseRepository wareHouseRepository, IHopRepository hopRepository, IWebhookRepository webhookRepository)
        {
            this.mapper = mapper;
            this.parcelRepository = parcelRepository;
            this.logger = logger;
            this.hopRepository = hopRepository;
            this.webhookRepository = webhookRepository;
            this.geoCodingAgent = geoCodingAgent;
            this.wareHouseRepository = wareHouseRepository;
            this.routeCalculator = routeCalculator;
        }

        public Parcel TrackParcel(string trackingId)
        {
            try
            {
                logger.LogInformation($"getting parcel {trackingId} from repo");
                Data.Parcel dataParcel = parcelRepository.GetByTrackingId(trackingId);
                Parcel businessParcel = this.mapper.Map<Parcel>(dataParcel);

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

                
                logger.LogDebug($"calculating route between sender {senderHop.Code} and recipeint {recipientHop.Code}");
                List<HopArrival> route = routeCalculator.CalculateRoute(warehouse, senderHop.Code, recipientHop.Code, businessParcel.EntryDate);
                

                //route    Datetime now
                List<HopArrival> soonHop = new List<HopArrival>();
                List<HopArrival> pastHop = new List<HopArrival>();
                foreach (HopArrival ha in route)
                {
                    if (ha.DateTime < DateTime.Now )
                    {
                        // beide listen befüllen
                        pastHop.Add(ha);
                /*
                        try
                        {
                            
                            logger.LogInformation($"updating parcel {trackingId}");
                            parcelRepository.Update(this.mapper.Map<Data.Parcel>(businessParcel));
                            List<Data.Webhook> dataWebhooks = webhookRepository.GetByTrackingId(trackingId);
                            List<Webhook> webhooks = new List<Webhook>();
                            dataWebhooks.ForEach(hook => webhooks.Add(this.mapper.Map<Webhook>(hook)));
                            NotifyAllSubscribers(webhooks);
                            
                        }
                        catch (DataAccessLayerException e)
                        {
                            throw new BusinessLayerException("DAL Exception", e);
                        }   
                */        
                    } else 
                    {
                        soonHop.Add(ha);
                    }
                }
                businessParcel.VisitedHops = pastHop;
                businessParcel.FutureHops = soonHop;
                
                return businessParcel;
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException();
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
    }
}
