using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class APIKeyValidatorSettings :IAPIKeyValidatorSettings
    {
        public bool Enabled { get; set; } = false;
        private Func<string/*apiKey*/, string/*method*/, string/*route*/, bool> _APIKeyIsValid { get; set; } = (_, _, _) => true;
        public bool APIKeyIsValid(string apiKey, string method, string route)
        {
            return _APIKeyIsValid(apiKey, method, route);
        }
        public APIKeyValidatorSettings(Func<string/*apiKey*/, string/*method*/, string/*route*/, bool> apiKeyIsValid)
        {
            this._APIKeyIsValid = apiKeyIsValid;
        }
    }
}