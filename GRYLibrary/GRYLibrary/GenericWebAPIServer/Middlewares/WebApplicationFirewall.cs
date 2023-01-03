using GRYLibrary.Core.GenericWebAPIServer.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a webapplicationfirewall
    /// </summary>
    public class WebApplicationFirewall : AbstractMiddleware
    {
        private readonly IWebApplicationFirewallSettings _WebApplicationFirewallSettings;
        /// <inheritdoc/>
        public WebApplicationFirewall(RequestDelegate next, IWebApplicationFirewallSettings webApplicationFirewallSettings) : base(next)
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

            return _Next(context);
        }
    }
}
