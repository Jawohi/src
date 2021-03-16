using Konya_Hiermayer.Packages.BL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL.Extensions
{
    public static class RecipientExtensions
    {
        public static string ToGeoCodingString(this Recipient recipient)
            => $"{recipient.Street}, {recipient.PostalCode} {recipient.City}";
    }
}
