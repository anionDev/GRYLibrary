using Microsoft.AspNetCore.Http;
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
            this._Next(context);
            this.Log(context);
            return Task.CompletedTask;
        }

        protected abstract void Log(HttpContext context);
    }
}