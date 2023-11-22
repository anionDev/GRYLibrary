using System;

namespace GRYLibrary.Core.Exceptions
{
    public class ServiceNotAvailableException : Exception
    {
        public ServiceNotAvailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}