using GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthenticationMiddleware :AuthenticationMiddleware
    {
        private readonly IKeyCloakAuthenticationMiddlewareSettings _AuthenticationMiddlewareSettings;
        private readonly IKeyCloakService _KeyCloakService;
        /// <inheritdoc/>
        public KeyCloakAuthenticationMiddleware(RequestDelegate next, IKeyCloakAuthenticationMiddlewareSettings authenticationMiddlewareSettings,IKeyCloakService keyCloak) : base(next)
        {
            this._AuthenticationMiddlewareSettings = authenticationMiddlewareSettings;
            this._KeyCloakService = keyCloak;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            //TODO
            return this._Next(context);
        }
    }
}
