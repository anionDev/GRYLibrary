using Microsoft.AspNetCore.Http;
using System;

namespace GRYLibrary.Core.APIServer.Services.CredC
{
    public static class CookieTools
    {
        public static string CookieName { get; set; } = "X-Authorization";
        //TODO move CookieName to configuration
        //TODO remove "X-"-prefix from CookieName
        public static (string key, string value, CookieOptions options) GetAccessTokenCookie(string username, string accessToken, DateTime expires) => GetCookieWithSpecificExpiredDate(username, expires, accessToken);

        public static (string key, string value, CookieOptions options) GetAccessTokenExpiredCookie(string username) => GetCookieWithSpecificExpiredDate(username, new DateTime(1970, 1, 1, 0, 0, 0), string.Empty);

        public static (string key, string value, CookieOptions options) GetCookieWithSpecificExpiredDate(string username, DateTime expiredDate, string accessToken) => (CookieName,
                $"User={username};AccessToken={accessToken}",
                new CookieOptions()
                {
                    Expires = expiredDate,
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                }
            );
    }
}
