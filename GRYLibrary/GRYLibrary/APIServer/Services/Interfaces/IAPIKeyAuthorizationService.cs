namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IAPIKeyAuthorizationService
    {
        public bool APIKeyIsValid(string actionName, string apiKey);
    }
}
