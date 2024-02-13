using Microsoft.AspNetCore.Http;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.Exceptions
{
    public class BadRequestException : UserFormattedException
    {
        public ushort HTTPStatusCode { get; }
        public BadRequestException(ushort httpStatusCode) : this(httpStatusCode, GetMessage(httpStatusCode))
        {
        }
        public BadRequestException(ushort httpStatusCode, string message) : base(message)
        {
            GUtilities.AssertCondition((httpStatusCode / 100) == 4);
            this.HTTPStatusCode = httpStatusCode;
        }

        private static string GetMessage(ushort httpStatusCode)
        {
            if (httpStatusCode == StatusCodes.Status401Unauthorized)
            {
                return "Authentication required";
            }
            else if (httpStatusCode == StatusCodes.Status403Forbidden)
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