using GRYLibrary.Core.APIServer.Services.KeyCloak;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.KCZAut
{
    public class KeyCloakAuthorizationService : IKeyCloakAuthorizationService
    {
        private readonly IKeyCloakService _KeyCloakService;
        public KeyCloakAuthorizationService(IKeyCloakService keyCloakService)
        {
            this._KeyCloakService = keyCloakService;
        }

        public void EnsureGroupDoesNotExist(string groupname)
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

        public ISet<string> GetGroupsOfUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public bool GroupExists(string groupname)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthorized(string user, string action)
        {
            throw new System.NotImplementedException();
        }

        public bool UserIsInGroup(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }
    }
}
