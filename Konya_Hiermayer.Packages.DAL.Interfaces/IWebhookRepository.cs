using System;
using System.Collections.Generic;
using Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.DAL.Interfaces
{
    public interface IWebhookRepository
    {
        void Create(Webhook webhook);

        void DeleteByWebhookId(long? id);

        List<Webhook> GetByTrackingId(string trackingId);
    }
}
