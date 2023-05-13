using GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthenticationMiddleware :AuthenticationMiddleware
    {
        private readonly IKeyCloakAuthenticationMiddlewareSettings _AuthenticationMiddlewareSettings;
        /// <inheritdoc/>
        public KeyCloakAuthenticationMiddleware(RequestDelegate next, IKeyCloakAuthenticationMiddlewareSettings authenticationMiddlewareSettings) : base(next)
        {
            this._AuthenticationMiddlewareSettings = authenticationMiddlewareSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            //TODO
            return this._Next(context);
        }
    }
}
