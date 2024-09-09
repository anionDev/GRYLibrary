using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Counter
{
    /// <summary>
    /// Represents a middleware which counts requests for paid Web-APIs.
    /// </summary>
    public abstract class RequestCounterMiddleware : AbstractMiddleware
    {
        /// <inheritdoc/>
        public RequestCounterMiddleware(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context) =>
            // TODO if response is 200 then add it to database or else return "429 Too Many Requests"

            this._Next(context);
    }
}