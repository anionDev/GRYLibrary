using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Exceptions
{
    public class InternalAlgorithmException : Exception
    {
        public InternalAlgorithmException(string errorId, string message = null) : base(CalculateMessage(errorId, message))
        {
        }

        private static string CalculateMessage(string errorId, string message)
        {
            var result = $"Internal alrogithm error. Error-id: {errorId}";
            if (message != null)
            {
                result = $"{result}; Message: {message}";
            }
            return result;
        }
    }
}