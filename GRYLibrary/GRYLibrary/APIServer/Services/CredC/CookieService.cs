﻿using Microsoft.AspNetCore.Http;
using System;

namespace GRYLibrary.Core.APIServer.Services.CredC
{
    public class CookieService : ICookieService
    {
        public ICookieServiceConfiguration CookieServiceConfiguration { get; set; }
        public CookieService(ICookieServiceConfiguration cookieServiceConfiguration)
        {
            this.CookieServiceConfiguration = cookieServiceConfiguration;
        }
        public virtual string ExtractSecret(HttpContext context)
        {
            this.TryGetCookieValue(context, out string result);
            return result;
        }

        public virtual bool ContainsCredentials(HttpContext context)
        {
            return this.TryGetCookieValue(context, out string _);
        }

        public virtual bool TryGetCookieValue(HttpContext context, out string cookie)
        {
            return context.Request.Cookies.TryGetValue(this.CookieServiceConfiguration.CookieName, out cookie);
        }

        public (string key, string value, CookieOptions options) CreateCookie(string username, string value, DateTime expiredMoment)
        {
            return CookieTools.GetAccessTokenCookie(username, value, expiredMoment);
        }

        public (string key, string value, CookieOptions options) GetAccessTokenExpiredCookie(string name)
        {
            return CookieTools.GetAccessTokenExpiredCookie(name);
        }
    }
}
