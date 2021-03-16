using System.Globalization;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;
using Konya_Hiermayer.Packages.DAL.Interfaces;

namespace Konya_Hiermayer.Packages.DAL.SQL
{
    public class WebhookRepository : IWebhookRepository
    {
        private readonly DatabaseContext context;
        private readonly ILogger<WebhookRepository> logger;

        public WebhookRepository(DatabaseContext context, ILogger<WebhookRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public void Create(Webhook webhook)
        {
            try
            {
                context.Webhooks.Add(webhook);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Duplicate Webhook (trackingId: {webhook.TrackingId}, url: {webhook.Url}) in DB");
                throw new DuplicateWebhookExpection();
            }
        }

        public void DeleteByWebhookId(long? id)
        {
            try
            {
                 Webhook webhook = new Webhook { Id = id };
                 context.Webhooks.Attach(webhook);
                 context.Webhooks.Remove(webhook);
                 context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Webhook with id {id} not found");
                throw new WebhookNotFoundExpection();
            }
        }

        public List<Webhook> GetByTrackingId(string trackingId)
        {
            try
            {
                return context.Webhooks.Where(hook => hook.TrackingId == trackingId).ToList();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"No Webhooks for Parcel with id {trackingId} found");
                throw new WebhookNotFoundExpection();
            }
        }
    }
}
