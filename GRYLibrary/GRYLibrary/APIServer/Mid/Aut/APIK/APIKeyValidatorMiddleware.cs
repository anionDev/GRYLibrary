using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Mid.Aut.APIK
{
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
        public override bool IsAuthorized(HttpContext context)
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