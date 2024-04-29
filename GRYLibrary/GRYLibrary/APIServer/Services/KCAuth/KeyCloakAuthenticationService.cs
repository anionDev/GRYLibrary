using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.KeyCloak;
using GRYLibrary.Core.Crypto;
using System.Collections.Generic;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

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

        public ISet<User> GetAllUser()
        {
            throw new System.NotImplementedException();
        }

        public ISet<string> GetGroupsOfUser(string username)
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

        public bool GroupExists(string groupname)
        {
            throw new System.NotImplementedException();
        }

        public string Hash(string password)
        {
            return GUtilities.ByteArrayToHexString(new SHA256().Hash(GUtilities.StringToByteArray(password)));
        }

        public AccessToken Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public void Logout(AccessToken accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void LogoutEverywhere(string username)
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

        public bool UserIsInGroup(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }
    }
}
