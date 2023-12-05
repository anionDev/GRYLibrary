using GRYLibrary.Core.APIServer.Services.KeyCloak;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Services.KCZAut
{
    public class KeyCloakAuthorizationService : IKeyCloakAuthorizationService
    {
        private readonly IKeyCloakService _KeyCloakService;
        public KeyCloakAuthorizationService(IKeyCloakService keyCloakService)
        {
            this._KeyCloakService = keyCloakService;
        }

        public void AssertIsAuthorized(string action, string user, string secret)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureGroupExists(string groupUser)
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

        public bool IsAuthorized(HttpContext context, Utilities.AuthorizeAttribute authorizeAttribute)
        {
            System.Collections.Generic.ISet<string> groups = authorizeAttribute.Groups;
            foreach (string group in groups)
            {
                if (this._KeyCloakService.UserIsInGroup(context.User.Identity.Name, group))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAuthorized(string action)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthorized(string action, string secret)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthorized(string action, string user, string secret)
        {
            throw new System.NotImplementedException();
        }

        public bool UserIsInGroup(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }
    }
}
