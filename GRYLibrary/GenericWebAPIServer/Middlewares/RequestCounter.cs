using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which counts requests for paid Web-APIs.
    /// </summary>
    public class RequestCounter : AbstractMiddleware
    {
        /// <inheritdoc>/>
        public RequestCounter(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc>/>
        public override Task Invoke(HttpContext context)
        {
            // TODO if response is 200 then add it to database

            return _Next(context);
        }
    }
}
