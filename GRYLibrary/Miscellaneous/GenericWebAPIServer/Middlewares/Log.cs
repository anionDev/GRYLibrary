using CryptoCurrencyOnlineTools.Library.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class Log : AbstractMiddleware
    {
        /// <inheritdoc>/>
        public Log(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc>/>
        public override Task Invoke(HttpContext context)
        {
            // TODO log request.route, request.sourceip, response.statuscode, duration of creating response (nothing else)

            return _Next(context);
        }
    }
}
