using GRYLibrary.Core.LogObject;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class Log : AbstractMiddleware
    {
        private readonly Action<Action<GRYLog>> _LogAction;
        /// <inheritdoc>/>
        public Log(RequestDelegate next, Action<Action<GRYLog>> logAction) : base(next)
        {
            this._LogAction = logAction;
        }
        /// <inheritdoc>/>
        public override Task Invoke(HttpContext context)
        {
            //this._LogAction(logObject=>logObject.Log("Some log"));

            // TODO log request.route, request.sourceip, response.statuscode, duration of creating response (nothing else)

            return _Next(context);
        }
    }
}
