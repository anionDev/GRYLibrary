using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.RLog
{
    /// <summary>
    /// Represents a middleware which logs the HTTP-requests.
    /// </summary>
    public abstract class RequestLoggingMiddleware : AbstractMiddleware
    {
        public RequestLoggingMiddleware(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            (byte[] requestBodyBytes, byte[] responseBodyBytes) = Tools.ExecuteNextMiddlewareAndGetRequestAndResponseBody(context, this._Next);
            string temp1 = new UTF8Encoding(false).GetString(requestBodyBytes);
            string temp2 = new UTF8Encoding(false).GetString(responseBodyBytes);
            this.Log(context, requestBodyBytes, responseBodyBytes);
            return Task.CompletedTask;
        }

        protected abstract void Log(HttpContext context, byte[] request, byte[] response);
    }
}