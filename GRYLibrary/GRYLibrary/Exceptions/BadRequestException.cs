using System.Net;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.Exceptions
{
    public class BadRequestException : UserFormattedException
    {
        public HttpStatusCode HttpStatusCode { get; }
        public BadRequestException(HttpStatusCode httpStatusCode) : this(httpStatusCode, GetMessage(httpStatusCode))
        {
        }
        public BadRequestException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            GUtilities.AssertCondition(((int)httpStatusCode / 100)==4);
            this.HttpStatusCode = httpStatusCode;
        }

        private static string GetMessage(HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.Unauthorized)
            {
                return "Authentication required";
            }
            else if (httpStatusCode == HttpStatusCode.Forbidden)
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