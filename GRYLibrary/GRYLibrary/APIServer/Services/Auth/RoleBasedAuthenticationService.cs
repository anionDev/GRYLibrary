using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Crypto;
using System;
using System.Collections.Generic;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    public class RoleBasedAuthenticationService : IAuthenticationService
    {
        private readonly IRoleBasedAuthenticationPersistence _RoleBasedAuthenticationPersistence;

        public RoleBasedAuthenticationService(IRoleBasedAuthenticationPersistence roleBasedAuthenticationPersistence)
        {
            this._RoleBasedAuthenticationPersistence = roleBasedAuthenticationPersistence;
        }

        public bool AccessTokenIsValid(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void EnsureGroupDoesNotExist(string groupname)
        {
            throw new NotImplementedException();
        }

        public void EnsureGroupExists(string groupUser)
        {
            throw new NotImplementedException();
        }

        public void EnsureUserIsInGroup(string username, string groupname)
        {
            throw new NotImplementedException();
        }

        public void EnsureUserIsNotInGroup(string username, string groupname)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetGroupsOfUser(string username)
        {
            throw new NotImplementedException();
        }

        public string GetIdOfUser(string username)
        {
            throw new NotImplementedException();
        }

        public string GetUserName(string accessToken)
        {
            throw new NotImplementedException();
        }

        public bool GroupExists(string groupname)
        {
            throw new NotImplementedException();
        }

        public AccessToken Login(string username, string password)
        {
            throw new NotImplementedException();
            //TODO also persist accesstoken which leads to AccessTokenIsValid(accesstoken)==true
        }

        public void Logout(AccessToken accessToken)
        {
            throw new NotImplementedException();
        }

        public void LogoutEverywhere(string username)
        {
            throw new NotImplementedException();
        }

        public void Register(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(string username)
        {
            throw new NotImplementedException();
        }

        public bool UserExists(string username)
        {
            throw new NotImplementedException();
        }

        public bool UserIsInGroup(string username, string groupname)
        {
            throw new NotImplementedException();
        }

        public string Hash(string password)
        {
            return GUtilities.ByteArrayToHexString(new SHA256().Hash(GUtilities.StringToByteArray(password)));
        }

        public ISet<User> GetAllUser()
        {
            throw new NotImplementedException();
        }
    }
}
