using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.KeyCloak;

namespace GRYLibrary.Core.APIServer.Services.KCZAuth
{
    public class KeyCloakAuthenticationService : IKeyCloakAuthenticationService
    {
        private readonly IKeyCloakService _KeyCloakService;
        private readonly IHTTPCredentialsProvider _HTTPCredentialsProvider;
        public KeyCloakAuthenticationService(IKeyCloakService keyCloakService, IHTTPCredentialsProvider httpCredentialsProvider)
        {
            this._KeyCloakService = keyCloakService;
            this._HTTPCredentialsProvider = httpCredentialsProvider;
        }

        public bool AccessTokenIsValid(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public string GetIdOfUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserName(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public AccessToken Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public void Logout(AccessToken accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void Register(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public bool UserExists(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
