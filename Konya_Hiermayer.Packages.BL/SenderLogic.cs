using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Konya_Hiermayer.Packages.BL.Entities.Validators;
using Konya_Hiermayer.Packages.BL.Extensions;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Konya_Hiermayer.Packages.ServiceAgents.Interfaces;
using Data = Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.BL
{
    public class SenderLogic : ISenderLogic
    {
        private readonly ILogisticsPartnerLogic logisticsPartnerLogic;
        private readonly ILogger<SenderLogic> logger;

        public SenderLogic(ILogisticsPartnerLogic logisticsPartnerLogic, ILogger<SenderLogic> logger)
        {
            this.logisticsPartnerLogic = logisticsPartnerLogic;
            this.logger = logger;
        }

        public string SubmitNewParcel(Parcel newParcel)
        {
            string generatedTrackingId = RandomString(9);
            logger.LogDebug($"generated trackingID {generatedTrackingId}");
            // Das hier throwed eine BL Exception: unbedingt catchen!!
            return logisticsPartnerLogic.TransferParcel(generatedTrackingId, newParcel);
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
