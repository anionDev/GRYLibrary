using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Mid.AuthS
{
    /// <summary>
    /// Represents an <see cref="AuthenticationMiddleware"/> which implements authentication-checks using <see cref="IAuthenticationService"/>.
    /// </summary>
    public class AuthSMiddleware : AuthenticationMiddleware
    {
        private readonly IAuthenticationService _AuthenticationService;
        public AuthSMiddleware(RequestDelegate next, IAuthenticationService authenticationService) : base(next)
        {
            this._AuthenticationService = authenticationService;
        }

        public override bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal)
        {
            return this._AuthenticationService.TryGetAuthentication(context, out principal);
        }
    }
}
