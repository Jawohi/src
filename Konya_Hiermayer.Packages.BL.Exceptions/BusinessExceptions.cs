using System;
using System.Collections;

namespace Konya_Hiermayer.Packages.BL.Exceptions
{
    public class BusinessException : Exception
    {
        protected string exceptionName;

        public BusinessException() { }
        public BusinessException(string message) : base(message) { }
    }

    public class ParcelNotFoundExpection : BusinessException
    {

        public ParcelNotFoundExpection()
        {
            base.exceptionName = this.GetType().Name;
        }
    }

    public class HopNotFoundExpection : BusinessException
    {

        public HopNotFoundExpection()
        {
            base.exceptionName = this.GetType().Name;
        }
    }

    public class InvalidParcelException : BusinessException
    {
        public InvalidParcelException(string message) : base(message)
        {
            base.exceptionName = this.GetType().Name;
        }
    }

    public class InvalidWarehouseException : BusinessException
    {
        public InvalidWarehouseException(string message) : base(message)
        {
            base.exceptionName = this.GetType().Name;
        }
    }
}
