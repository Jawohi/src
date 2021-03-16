using System;
using System.Collections.Generic;
using System.Text;

namespace Konya_Hiermayer.Packages.BL.Entities.Exceptions
{
    public class BusinessLayerException : Exception
    {
        public BusinessLayerException() { }
        public BusinessLayerException(string message) : base(message) { }
        public BusinessLayerException(string message, Exception e) : base(message, e) { }
    }

    public class InvalidParcelException : BusinessLayerException
    {
        public InvalidParcelException(string message) : base(message)
        { }
    }

    public class InvalidWarehouseException : BusinessLayerException
    {
        public InvalidWarehouseException(string message) : base(message)
        { }
    }

    public class NoHopException : BusinessLayerException
    {
        public NoHopException(string message) : base(message)
        { }
    }
}
