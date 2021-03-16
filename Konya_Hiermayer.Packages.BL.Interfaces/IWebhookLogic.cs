using System;
using System.Collections.Generic;
using Konya_Hiermayer.Packages.BL.Entities;

namespace Konya_Hiermayer.Packages.BL.Interfaces
{
    public interface IWebhookLogic
    {
        List<Webhook> GetAllSubscriptionsFromParcel(string trackingId);
        void SubscribeToParcel(string trackingId, string url);
        void UnsubscribeFromParcel(long? id);
    }
}
