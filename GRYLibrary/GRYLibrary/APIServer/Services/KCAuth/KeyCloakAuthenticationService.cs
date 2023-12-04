using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.Services.KeyCloak;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Services.KCZAuth
{
    public class KeyCloakAuthorizationService : IKeyCloakAuthenticationService
    {
        private readonly IKeyCloakService _KeyCloakService;
        public KeyCloakAuthorizationService(IKeyCloakService keyCloakService)
        {
            _KeyCloakService = keyCloakService;
        }
        public bool AccessTokenIsValid(string username, string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureUserIsInGroup(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureUserIsNotInGroup(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }

        public bool GroupExists(string groupname)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthenticated(HttpContext context, AuthorizeAttribute authorizeAttribute)
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

        public bool UserIsInGroup(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }
    }
}
