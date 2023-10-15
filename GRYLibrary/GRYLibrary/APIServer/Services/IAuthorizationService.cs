namespace GRYLibrary.Core.APIServer.Services
{
    public interface IAuthorizationService
    {
        public bool APIKeyIsValid(string actionName, string apiKey);
    }
}
