using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Data = Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.BL
{
    public class WebhookLogic : IWebhookLogic
    {
        private readonly IMapper mapper;
        private readonly IWebhookRepository webhookRepository;
        private readonly ILogger<WebhookLogic> logger;
        private readonly IParcelRepository parcelRepository;

        public WebhookLogic(IMapper mapper, IWebhookRepository webhookRepository, IParcelRepository parcelRepository,  ILogger<WebhookLogic> logger)
        {
            this.mapper = mapper;
            this.webhookRepository = webhookRepository;
            this.parcelRepository = parcelRepository;
            this.logger = logger;
        }
        public List<Webhook> GetAllSubscriptionsFromParcel(string trackingId)
        {
            try
            {
                logger.LogDebug($"Getting all webhooks with trackingId {trackingId}");
                List<Data.Webhook> dataWebhooks = webhookRepository.GetByTrackingId(trackingId);
                
                List<Webhook> businessWebhooks = new List<Webhook>();
                logger.LogDebug($"Converting all DA.webhooks to BL.webhooks (all webhooks with trackingId {trackingId}");
                dataWebhooks.ForEach(webhook => businessWebhooks.Add(this.mapper.Map<Webhook>(webhook)));
                return businessWebhooks;
            }
            catch(DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }
        }

        public void SubscribeToParcel(string trackingId, string url)
        {
            try
            {
                logger.LogDebug($"Checking if Parcel with trackingId {trackingId} exists");
                parcelRepository.GetByTrackingId(trackingId); // if it doesnt throw an error it found a parcel

                DateTime currentTime = DateTime.Now;
                Data.Webhook webhook = new Data.Webhook() { TrackingId = trackingId, Url = url, CreatedAt = currentTime };
                logger.LogDebug($"Creating entry for webhook with trackingId: {trackingId}, url: {url}, createdAt: {currentTime}");
                webhookRepository.Create(webhook);
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }
        }

        public void UnsubscribeFromParcel(long? id)
        {
            try
            {
                logger.LogDebug($"Unsubscribing hook with id {id}");
                webhookRepository.DeleteByWebhookId(id);
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }
        }
    }
}
