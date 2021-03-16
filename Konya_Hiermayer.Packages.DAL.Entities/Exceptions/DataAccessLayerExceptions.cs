using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.DAL.Entities.Exceptions
{
    public class DataAccessLayerException : Exception
    {
        protected string exceptionName;

        public DataAccessLayerException() { }
        public DataAccessLayerException(string message) : base(message) { }
    }

    public class ResourceNotFoundException : DataAccessLayerException
    {
        public ResourceNotFoundException(string message) : base(message)
        { }
    }

    public class DuplicateParcelExpection : DataAccessLayerException { }
    public class ParcelNotFoundExpection : DataAccessLayerException { }

    public class DuplicateHopExpection : DataAccessLayerException { }
    public class HopNotFoundExpection : DataAccessLayerException { }

    public class DuplicateWarehouseExpection : DataAccessLayerException { }
    public class WarehouseNotFoundExpection : DataAccessLayerException { }

    public class DuplicateWebhookExpection : DataAccessLayerException { }
    public class WebhookNotFoundExpection : DataAccessLayerException { }
}
