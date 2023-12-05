using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.Services.KeyCloak;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Services.KCZAuth
{
    public class KeyCloakAuthenticationService : IKeyCloakAuthenticationService
    {
        private readonly IKeyCloakService _KeyCloakService;
        public KeyCloakAuthenticationService(IKeyCloakService keyCloakService)
        {
            this._KeyCloakService = keyCloakService;
        }

        public bool AccessTokenIsValid(string username, string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public UserBackendInformation GetUserByName(string username)
        {
            throw new System.NotImplementedException();
        }

        public AccessToken Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public void Logout(string name)
        {
            throw new System.NotImplementedException();
        }

        public void OnStart()
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

        public bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal)
        {
            throw new System.NotImplementedException();
        }

        public bool UserExists(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
