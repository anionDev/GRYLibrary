using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Exceptions
{
    public class ServiceNotAvailableException : Exception
    {
        public ServiceNotAvailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}