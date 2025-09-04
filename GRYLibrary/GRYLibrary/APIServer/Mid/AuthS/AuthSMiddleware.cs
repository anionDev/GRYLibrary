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
        public AuthSMiddleware(RequestDelegate next, ICredentialsProvider credentialsProvider, IAuthenticationService authenticationService, IAuthSConfiguration authenticationConfiguration) : base(next, authenticationConfiguration, authenticationService)
        {
        }

    }
}
