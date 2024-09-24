using GRYLibrary.Core.APIServer.MidT.Auth;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Mid.NewFolder
{
    /// <summary>
    /// Represents an <see cref="AuthorizationMiddleware"/> which implements authorizaton-checks using API-keys.
    /// </summary>
    public abstract class APIKeyValidatorMiddleware : AuthorizationMiddleware
    {
        protected readonly IAPIKeyValidatorConfiguration _APIKeyValidatorSettings;
        public APIKeyValidatorMiddleware(RequestDelegate next, IAPIKeyValidatorConfiguration apiKeyValidatorSettings) : base(next)
        {
            this._APIKeyValidatorSettings = apiKeyValidatorSettings;
        }

        public override bool AuthorizationIsRequired(HttpContext context)
        {
            return false;//API-keys only check for authorization, not for authentication.
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