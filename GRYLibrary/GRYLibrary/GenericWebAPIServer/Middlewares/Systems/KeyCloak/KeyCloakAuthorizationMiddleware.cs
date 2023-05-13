using GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthorizationMiddleware :AuthorizationMiddleware
    {
        private readonly IKeyCloakAuthorizationMiddlewareSettings _IAuthorizationMiddlewareSettings;
        /// <inheritdoc/>
        public KeyCloakAuthorizationMiddleware(RequestDelegate next, IKeyCloakAuthorizationMiddlewareSettings authorizationMiddlewareSettings) : base(next)
        {
            this._IAuthorizationMiddlewareSettings = authorizationMiddlewareSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            //TODO
            return this._Next(context);
        }
    }
}
