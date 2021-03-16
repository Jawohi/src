using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.Services.Models.Exceptions
{
    public class ServiceLayerException : Exception
    {
        protected string exceptionName;

        public ServiceLayerException() { }
        public ServiceLayerException(string message) : base(message) { }
        public ServiceLayerException(string message, Exception e) : base(message, e) { }
    }
}
