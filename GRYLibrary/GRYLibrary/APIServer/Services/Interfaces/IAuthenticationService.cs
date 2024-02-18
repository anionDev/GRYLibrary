using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public void Register(string username, string password);
        public AccessToken Login(string username, string password);
        public bool AccessTokenIsValid(string accessToken);
        /// <remarks>
        /// This operation does not check if the <paramref name="accessToken"/> is valid.
        /// </remarks>
        public string GetUserName(string accessToken);
        public string GetIdOfUser(string username);
        void Logout(AccessToken accessToken);
        public void RemoveUser(string username);
        bool UserExists(string username);
    }
}
