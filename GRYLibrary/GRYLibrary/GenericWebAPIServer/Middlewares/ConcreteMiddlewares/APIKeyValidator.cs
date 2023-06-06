using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    public class APIKeyValidator :AbstractMiddleware
    {
        protected readonly IAPIKeyValidatorSettings _APIKeyValidatorSettings;
        public APIKeyValidator(RequestDelegate next, IAPIKeyValidatorSettings apiKeyValidatorSettings) : base(next)
        {
            this._APIKeyValidatorSettings = apiKeyValidatorSettings;
        }

        public override Task Invoke(HttpContext context)
        {
            string method = context.Request.Method;
            string route = context.Request.Path;
            bool accessAllowed;
            if(this._APIKeyValidatorSettings.APIKeyIsRequired(method, route))
            {
                (bool provided, string apiKey) = this._APIKeyValidatorSettings.TryGetAPIKey(context);
                if(provided)
                {
                    accessAllowed = this._APIKeyValidatorSettings.APIKeyIsAuthorized(apiKey, method, route);
                    if(accessAllowed)
                    {
                        context.Items["APIKey"] = apiKey;
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                    }
                }
                else
                {
                    accessAllowed = false;
                    context.Response.StatusCode = 403;
                }
            }
            else
            {
                accessAllowed = true;
            }
            if(accessAllowed)
            {
                return this._Next(context);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}