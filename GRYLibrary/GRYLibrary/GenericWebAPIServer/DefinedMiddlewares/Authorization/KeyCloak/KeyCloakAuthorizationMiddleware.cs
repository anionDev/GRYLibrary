using GRYLibrary.Core.GenericWebAPIServer.Services.KeyCloak;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authorization.KeyCloak
{
    public class KeyCloakAuthorizationMiddleware :AuthorizationMiddleware
    {
        private readonly IKeyCloakAuthorizationMiddlewareConfiguration _IAuthorizationMiddlewareSettings;
        private readonly IKeyCloakService _KeyCloakService;
        /// <inheritdoc/>
        public KeyCloakAuthorizationMiddleware(RequestDelegate next, IKeyCloakAuthorizationMiddlewareConfiguration authorizationMiddlewareSettings, IKeyCloakService keyCloakService) : base(next, authorizationMiddlewareSettings)
        {
            this._IAuthorizationMiddlewareSettings = authorizationMiddlewareSettings;
            this._KeyCloakService = keyCloakService;
        }

        public override bool IsAuthorized(HttpContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
