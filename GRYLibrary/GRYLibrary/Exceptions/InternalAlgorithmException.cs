using System;

namespace GRYLibrary.Core.Exceptions
{
    public class InternalAlgorithmException : Exception
    {
        public InternalAlgorithmException() : base()
        {
        }

        public InternalAlgorithmException(string errorId) : base(CalculateMessage(errorId))
        {
        }

        public InternalAlgorithmException(string errorId, string message ) : base(CalculateMessage(errorId, message))
        {
        }

        private static string CalculateMessage(string message)
        {
            string result = $"Internal alrogithm error.";
            if (message != null)
            {
                result = $"{result}; Message: {message}";
            }
            return result;
        }

        private static string CalculateMessage(string errorId, string message)
        {
            string result = $"Internal alrogithm error. Error-information: {errorId}";
            if (message != null)
            {
                result = $"{result}; Message: {message}";
            }
            return result;
        }
    }
}