﻿using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.WebApplicationFirewall
{
    /// <summary>
    /// Represents a webapplicationfirewall
    /// </summary>
    public class WebApplicationFirewallMiddleware :AbstractMiddleware
    {
        private readonly IWebApplicationFirewallConfiguration _WebApplicationFirewallSettings;
        /// <inheritdoc/>
        public WebApplicationFirewallMiddleware(RequestDelegate next, IWebApplicationFirewallConfiguration webApplicationFirewallSettings) : base(next)
        {
            this._WebApplicationFirewallSettings = webApplicationFirewallSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            // TODO log & block (by default) request when
            // - the route or the payload contains some "strange" context (e.g. only one single quote or something like this (rules/exceptions must be definable for specific routes)) or
            // - the json-/xml-payload is syntactically invalid or
            // - an xml-payload uses external entities or
            // - the response is much longer than expected/allowed (must be configured by the application which is using this middleware)
            // and make this configurable

            return this._Next(context);
        }
    }
}