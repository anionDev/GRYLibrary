using GRYLibrary.Core.APIServer.Services.KeyCloak;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Mid.Auth.KC
{
    public class KeyCloakAuthenticationMiddleware : AuthenticationMiddleware
    {
        private readonly IKeyCloakAuthenticationConfiguration _AuthenticationMiddlewareSettings;
        private readonly IKeyCloakService _KeyCloakService;
        /// <inheritdoc/>
        public KeyCloakAuthenticationMiddleware(RequestDelegate next, IKeyCloakAuthenticationConfiguration authenticationMiddlewareSettings, IKeyCloakService keyCloak) : base(next, authenticationMiddlewareSettings)
        {
            this._AuthenticationMiddlewareSettings = authenticationMiddlewareSettings;
            this._KeyCloakService = keyCloak;
        }

        public override bool AuthenticatedIsRequired(HttpContext context)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal)
        {
            throw new System.NotImplementedException();
        }
    }
}
