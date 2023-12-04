namespace GRYLibrary.Core.APIServer.Services.APIK
{
    public class APIKeyAuthorizationService : IAPIKeyAuthorizationService
    {
        private readonly IAPIKeyAuthorizationConfiguration _APIKeyAuthorizationConfiguration;
        public APIKeyAuthorizationService(IAPIKeyAuthorizationConfiguration apiKeyAuthorizationConfiguration)
        {
            _APIKeyAuthorizationConfiguration = apiKeyAuthorizationConfiguration;
        }
        public bool IsAuthorized(string action, string apiKey)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthorized(string action)
        {
            throw new System.NotSupportedException();
        }
    }
}
