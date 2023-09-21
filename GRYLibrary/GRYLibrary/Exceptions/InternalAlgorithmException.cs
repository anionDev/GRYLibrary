using System;

namespace GRYLibrary.Core.Exceptions
{
    public class InternalAlgorithmException : Exception
    {
        public InternalAlgorithmException() : base()
        {
        }

        public InternalAlgorithmException(string errorId, string message = null) : base(CalculateMessage(errorId, message))
        {
        }

        private static string CalculateMessage(string errorId, string message)
        {
            string result = $"Internal alrogithm error. Error-id: {errorId}";
            if (message != null)
            {
                result = $"{result}; Message: {message}";
            }
            return result;
        }
    }
}