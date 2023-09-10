using System;

namespace GRYLibrary.Core.Exceptions
{
    public class APIKeyLimitExceeded : Exception
    {
        public APIKeyLimitExceeded() : this("API-Key limit exceeded.")
        {
        }
        public APIKeyLimitExceeded(string message) : base(message)
        {
        }
    }
}