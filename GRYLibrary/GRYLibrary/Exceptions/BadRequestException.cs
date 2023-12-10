using System.Net;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.Exceptions
{
    public class BadRequestException : UserFormattedException
    {
        public uint HTTPStatusCode { get; }
        public BadRequestException(uint httpStatusCode) : this(httpStatusCode, GetMessage(httpStatusCode))
        {
        }
        public BadRequestException(uint httpStatusCode, string message) : base(message)
        {
            GUtilities.AssertCondition((httpStatusCode / 100)==4);
            this.HTTPStatusCode = httpStatusCode;
        }

        private static string GetMessage(uint httpStatusCode)
        {
            if (httpStatusCode == (int)HttpStatusCode.Unauthorized)
            {
                return "Authentication required";
            }
            else if (httpStatusCode == (int)HttpStatusCode.Forbidden)
            {
                return "Unauthorized";
            }
            else
            {
                return "Bad request";
            }
        }
    }
}