using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.RLog
{
    /// <summary>
    /// Represents a middleware which logs the HTTP-requests.
    /// </summary>
    public abstract class RequestLoggingMiddleware : AbstractMiddleware
    {
        private readonly ITimeService _TimeService;
        public RequestLoggingMiddleware(RequestDelegate next, ITimeService timeService) : base(next)
        {
            this._TimeService = timeService;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            try
            {
                DateTime begin = this._TimeService.GetCurrentTime();
                (byte[] requestBodyBytes, byte[] responseBodyBytes) = Tools.ExecuteNextMiddlewareAndGetRequestAndResponseBody(context, this._Next);
                DateTime end = this._TimeService.GetCurrentTime();
                TimeSpan duration = end - begin;
                context.Items["Duration"] = duration;
                //TODO provide resposne-status-code and duration also as metrics.
                this.Log(context, requestBodyBytes, responseBodyBytes);
                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }

        protected abstract void Log(HttpContext context, byte[] request, byte[] response);
    }
}