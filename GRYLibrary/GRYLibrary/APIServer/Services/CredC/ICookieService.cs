﻿using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace GRYLibrary.Core.APIServer.Services.Cred
{
    public interface ICookieService : ICredentialsProvider
    {
        public abstract bool TryGetCookieValue(HttpContext context, out string cookie);
        (string key, string value, CookieOptions options) CreateCookie(string username, string value, DateTime expiredMoment);
        (string key, string value, CookieOptions options) GetAccessTokenExpiredCookie(string name);
    }
}
