using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.APIK;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Mid.AuthS
{
    /// <summary>
    /// Represents an <see cref="APIKeyAuthorizationService"/> which implements authorizaton-checks using group-memberships defined by <see cref="IAuthenticationService"/>.
    /// </summary>
    public class AuthSMiddleware : AuthenticationMiddleware
    {
        private readonly IAuthenticationService _AuthenticationService;
        protected AuthSMiddleware(RequestDelegate next, IAuthenticationService authenticationService) : base(next)
        {
            this._AuthenticationService = authenticationService;
        }

        public override bool IsAuthorized(HttpContext context, out ClaimsPrincipal principal)
        {
            bool result = _AuthenticationService.TryGetAuthentication(context, out principal);
            if (result)
            {
                context.User = principal;
            }
            return result;
        }
    }
}
