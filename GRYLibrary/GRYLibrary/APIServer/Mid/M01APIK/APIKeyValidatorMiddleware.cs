﻿using GRYLibrary.Core.APIServer.MidT.Aut;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Mid.M01APIK
{
    /// <summary>
    /// Represents an <see cref="AuthorizationMiddleware"/> which implements authorizaton-checks using API-keys.
    /// </summary>
    public abstract class APIKeyValidatorMiddleware : AuthorizationMiddleware
    {
        protected readonly IAPIKeyValidatorConfiguration _APIKeyValidatorSettings;
        public APIKeyValidatorMiddleware(RequestDelegate next, IAPIKeyValidatorConfiguration apiKeyValidatorSettings) : base(next, apiKeyValidatorSettings)
        {
            this._APIKeyValidatorSettings = apiKeyValidatorSettings;
        }

        public virtual (bool provided, string apiKey) TryGetAPIKey(HttpContext context)
        {
            return APIKeyValidatorFilter.TryGetAPIKey(context);
        }

        public abstract bool APIKeyIsAuthorized(string apiKey, HttpContext context);
        protected override bool IsAuthorized(HttpContext context)
        {
            (bool provided, string apiKey) = this.TryGetAPIKey(context);
            if (provided)
            {
                context.Items["APIKey"] = apiKey;
                return this.APIKeyIsAuthorized(apiKey, context);
            }
            else
            {
                return false;
            }
        }
    }
}