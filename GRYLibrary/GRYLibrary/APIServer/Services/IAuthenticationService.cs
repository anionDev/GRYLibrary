namespace GRYLibrary.Core.APIServer.Services
{
    public interface IAuthenticationService
    {
        public bool AccessTokenIsValid(string actionName, string accessToken, string username);
        public void Register(string username, string password,bool enabled);
        /// <returns>
        /// Returns an access-token.
        /// </returns>
        public string Login(string username, string password);
    }
}
