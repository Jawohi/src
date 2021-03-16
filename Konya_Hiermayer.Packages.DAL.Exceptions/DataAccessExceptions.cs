using System;

namespace Konya_Hiermayer.Packages.DAL.Exceptions
{
    public class DataAccessExcpetion : Exception
    {
        protected string exceptionName;

        public DataAccessExcpetion() { }
        public DataAccessExcpetion(string message) : base(message) { }
    }
 
}
