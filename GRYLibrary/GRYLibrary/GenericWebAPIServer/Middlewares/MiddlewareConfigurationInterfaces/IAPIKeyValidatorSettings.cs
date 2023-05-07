using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces
{
    public interface IAPIKeyValidatorSettings :IMiddlewareSettings
    {
        public bool APIKeyIsRequired(string method, string route);
        public (bool provided, string apiKey) TryGetAPIKey(HttpContext context);
        public bool APIKeyIsAuthorized(string apiKey, string method, string route);
    }
}