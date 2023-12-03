using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;

namespace GRYLibrary.Core.APIServer.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Checks if the <paramref name="accessToken"/> is a valid authentication for <paramref name="username"/>.
        /// </summary>
        public bool AccessTokenIsValid(string username, string accessToken);
        public void Register(string username, string password);
        public AccessToken Login(string username, string password);
    }
}
