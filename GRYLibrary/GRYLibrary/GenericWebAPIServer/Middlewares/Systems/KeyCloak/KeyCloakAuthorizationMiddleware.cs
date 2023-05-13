using GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthorizationMiddleware :AuthorizationMiddleware
    {
        private readonly IKeyCloakAuthorizationMiddlewareSettings _IAuthorizationMiddlewareSettings;
        private readonly IKeyCloakService _KeyCloakService;
        /// <inheritdoc/>
        public KeyCloakAuthorizationMiddleware(RequestDelegate next, IKeyCloakAuthorizationMiddlewareSettings authorizationMiddlewareSettings, IKeyCloakService keyCloakService) : base(next)
        {
            this._IAuthorizationMiddlewareSettings = authorizationMiddlewareSettings;
            this._KeyCloakService = keyCloakService;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            //TODO
            return this._Next(context);
        }
    }
}
