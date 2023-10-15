namespace GRYLibrary.Core.APIServer.Services
{
    public interface IAuthenticationService
    {
        public bool AccessTokenIsValid(string actionName, string accessToken, string username);
    }
}
