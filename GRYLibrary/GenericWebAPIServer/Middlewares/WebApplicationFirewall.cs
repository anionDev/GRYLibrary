using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a webapplicationfirewall
    /// </summary>
    public class WebApplicationFirewall : AbstractMiddleware
    {
        /// <inheritdoc>/>
        public WebApplicationFirewall(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc>/>
        public override Task Invoke(HttpContext context)
        {
            // TODO log & block request when
            // - the route or the payload contains some "strange" context (e.g. only one single quote or something like this (rules/exceptions must be definable for specific routes)) or
            // - the json-/xml-payload is syntactically invalid or
            // - an xml-payload uses external entities or
            // - the response is much longer than expected/allowed (must be configured by the application which is using this middleware)
            
            return _Next(context);
        }
    }
}
