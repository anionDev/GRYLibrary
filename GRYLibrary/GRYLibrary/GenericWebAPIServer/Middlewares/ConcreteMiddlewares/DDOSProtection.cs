using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    /// <summary>
    /// Represents a DDOS-proection.
    /// </summary>
    public class DDOSProtection :AbstractMiddleware
    {
        private readonly IDDOSProtectionSettings _DDOSProtectionSettings;
        /// <inheritdoc/>
        public DDOSProtection(RequestDelegate next, IDDOSProtectionSettings ddosProtectionSettings) : base(next)
        {
            this._DDOSProtectionSettings = ddosProtectionSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            // TODO create a self-learning database and block undesired traffic due to configuration
            // (for example block a request (using a appropriate response-code) when
            // - a source-ip has done more than 61 [configurable] requests in the last 60 seconds or
            // - the request comes from the darknet or
            // - or the request comes from bogon-land or
            // - the source-ip has too many concurrent connections or
            // - the source-ip is black-listed or
            // - the request is too big or
            // - the source-ip tries to do enumeration in the context of penetration-testing/hacking or 
            // - the source-ip has already done this request with the same route/payload in the last 2 seconds

            return this._Next(context);
        }
    }
}