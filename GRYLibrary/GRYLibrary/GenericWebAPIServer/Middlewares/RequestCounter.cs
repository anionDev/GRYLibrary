using GRYLibrary.Core.GenericWebAPIServer.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which counts requests for paid Web-APIs.
    /// </summary>
    public class RequestCounter :AbstractMiddleware
    {
        private readonly IRequestCounterSettings _RequestCounterSettings;
        /// <inheritdoc/>
        public RequestCounter(RequestDelegate next, IRequestCounterSettings requestCounterSettings) : base(next)
        {
            this._RequestCounterSettings = requestCounterSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            // TODO if response is 200 then add it to database or else return "429 Too Many Requests"

            return this._Next(context);
        }
    }
}