using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Mid.AuthS
{
    /// <summary>
    /// Represents an <see cref="AuthenticationMiddleware"/> which implements authentication-checks using <see cref="IAuthenticationService"/>.
    /// </summary>
    public class AuthSMiddleware : AuthenticationMiddleware
    {
        private readonly ICredentialsProvider _CredentialsProvider;
        private readonly IAuthenticationService _AuthenticationService;
        public AuthSMiddleware(RequestDelegate next, ICredentialsProvider credentialsProvider, IAuthenticationService authenticationService) : base(next)
        {
            this._CredentialsProvider = credentialsProvider;
            this._AuthenticationService = authenticationService;
        }

        public override bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal)
        {
            return Tools.TryGetAuthentication(this._CredentialsProvider, this._AuthenticationService, context, out principal);
        }
    }
}
