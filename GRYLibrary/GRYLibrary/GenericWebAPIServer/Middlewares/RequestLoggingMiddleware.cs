using GRYLibrary.Core.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class RequestLoggingMiddleware : AbstractMiddleware
    {
        private readonly Action<Action<GRYLog>> _LogAction;
        /// <inheritdoc/>
        public RequestLoggingMiddleware(RequestDelegate next, Action<Action<GRYLog>> logAction) : base(next)
        {
            _LogAction = logAction;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            //this._LogAction(logObject=>logObject.Log("Some log"));

            // TODO log request.route, request.sourceip, response.statuscode, duration of creating response

            return _Next(context);
        }
        public virtual bool LogFullEntry(string route, ushort responseStatusCode)
        {
            return responseStatusCode % 100 == 5;
        }
        public virtual LogLevel GetLogLevelForRequestLogEntry(string route, ushort responseStatusCode)
        {
            if (responseStatusCode % 100 == 5)
            {
                return LogLevel.Error;
            }
            else
            {
                return LogLevel.Information;
            }
        }
        public virtual ushort GetEventLogIdForRequestLogEntry(string route, ushort responseStatusCode)
        {
            return 12000;
        }
    }
}
