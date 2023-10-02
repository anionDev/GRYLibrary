using GRYLibrary.Core.APIServer.Services.KeyCloak;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Mid.Auth.KC
{
    public class KeyCloakAuthentication : AuthenticationMiddleware
    {
        private readonly IKeyCloakAuthenticationConfiguration _AuthenticationMiddlewareSettings;
        private readonly IKeyCloakService _KeyCloakService;
        /// <inheritdoc/>
        public KeyCloakAuthentication(RequestDelegate next, IKeyCloakAuthenticationConfiguration authenticationMiddlewareSettings, IKeyCloakService keyCloak) : base(next, authenticationMiddlewareSettings)
        {
            this._AuthenticationMiddlewareSettings = authenticationMiddlewareSettings;
            this._KeyCloakService = keyCloak;
        }

        public override bool AuthenticatedIsRequired(HttpContext context)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsAuthenticated(HttpContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
