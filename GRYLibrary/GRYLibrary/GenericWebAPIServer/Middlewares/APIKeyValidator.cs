using GRYLibrary.Core.GenericWebAPIServer.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public class APIKeyValidator :AbstractMiddleware
    {
        private readonly IAPIKeyValidatorSettings _APIKeyValidatorSettings;
        public APIKeyValidator(RequestDelegate next, IAPIKeyValidatorSettings apiKeyValidatorSettings) : base(next)
        {
            this._APIKeyValidatorSettings = apiKeyValidatorSettings;
        }

        public override Task Invoke(HttpContext context)
        {
            string apiKey = null;//TODO
            string route = null;//TODO
            if(!this.AnonymousAccessIsAllowed(this._APIKeyValidatorSettings, apiKey, route) && !this.UserIsAuthenticated())
            {
                throw new NotImplementedException();//return 401 Unauthorized
            }
            if(!this.APIKeyIsValid(this._APIKeyValidatorSettings, apiKey, route))
            {
                throw new NotImplementedException();//return 403 Forbidden
            }
            return this._Next(context);
        }

        private bool UserIsAuthenticated()
        {
            return true;//TODO
        }

        private bool AnonymousAccessIsAllowed(IAPIKeyValidatorSettings aPIKeyValidatorSettings, string apiKey, string route)
        {
            return true;//TODO
        }

        private bool APIKeyIsValid(IAPIKeyValidatorSettings apiKeyValidatorSettings, string apiKey, string route)
        {
            return true;//TODO
        }
    }
}