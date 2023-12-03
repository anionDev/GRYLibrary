using GRYLibrary.Core.APIServer.Services;
using GRYLibrary.Core.APIServer.Services.KeyCloak;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Mid.Auth.KC
{
    public class TransientKeyCloakAuthenticationMiddleware : AuthenticationMiddleware
    {
        private readonly IAuthenticationService _Service;
        public const string CookieName = "x-AccessToken";
        /// <inheritdoc/>
        public TransientKeyCloakAuthenticationMiddleware(RequestDelegate next, IAuthenticationService keyCloak, IAuthenticationConfiguration authenticationConfiguration) : base(next, authenticationConfiguration)
        {
            this._Service = keyCloak;
        }

        public override bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal)
        {
            principal = default;
            try
            {
                if (context.Request.Cookies.TryGetValue(CookieName, out string value))
                {
                    string[] splitted = value.Split(';');
                    string username = splitted[0].Split('=')[1];
                    string accessToken = splitted[1].Split('=')[1];
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        return false;
                    }
                    else
                    {
                        bool accessTokenIsValid = this._Service.AccessTokenIsValid(username, accessToken);
                        if (accessTokenIsValid)
                        {
                            principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, username), }, "Basic"));
                            return true;//TODO implement authentocation
                        }
                    }
                }
            }
            catch
            {
                GUtilities.NoOperation();
            }
            return false;
        }

        public static (string key, string value, CookieOptions options) GetAccessTokenCookie(string username, string accessToken, DateTime expires)
        {
            return (CookieName,
                $"User={username};AccessToken={accessToken}",
                new CookieOptions()
                {
                    Expires = expires,
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                }
            );
        }

        public static (string key, string value, CookieOptions options) GetAccessTokenExpiredCookie(string username)
        {
            return (CookieName,
                $"User={username};AccessToken=",
                new CookieOptions()
                {
                    Expires = new DateTime(1970, 1, 1, 0, 0, 0),
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                }
            );
        }
    }
}
