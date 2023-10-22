namespace GRYLibrary.Core.APIServer.Services
{
    public interface IAPIKeyAuthorizationService
    {
        public bool APIKeyIsValid(string actionName, string apiKey);
    }
}
