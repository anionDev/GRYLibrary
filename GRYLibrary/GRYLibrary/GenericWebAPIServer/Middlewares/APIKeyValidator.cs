using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using GRYLibrary.Core.GenericWebAPIServer.Services;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public class APIKeyValidator : AbstractMiddleware
    {
        private readonly IAPIKeyValidatorSettings _APIKeyValidatorSettings;
        public APIKeyValidator(RequestDelegate next, IAPIKeyValidatorSettings apiKeyValidatorSettings) : base(next)
        {
            _APIKeyValidatorSettings = apiKeyValidatorSettings;
        }

        public override Task Invoke(HttpContext context)
        {
            string apiKey = null;//TODO
            string route = null;//TODO
            if (!AnonymousAccessIsAllowed(_APIKeyValidatorSettings, apiKey, route) && !UserIsAuthenticated())
            {
                throw new NotImplementedException();//return 401 Unauthorized
            }
            if (!APIKeyIsValid(_APIKeyValidatorSettings, apiKey, route))
            {
                throw new NotImplementedException();//return 403 Forbidden
            }
            return _Next(context);
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
