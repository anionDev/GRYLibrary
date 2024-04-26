using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public string Hash(string password);
        public void Register(string username, string password);
        public AccessToken Login(string username, string password);
        public bool AccessTokenIsValid(string accessToken);
        /// <remarks>
        /// This operation does not check if the <paramref name="accessToken"/> is valid.
        /// </remarks>
        public string GetUserName(string accessToken);
        public string GetIdOfUser(string username);
        void Logout(AccessToken accessToken);
        void LogoutEverywhere(string username);
        public void RemoveUser(string username);
        bool UserExists(string username);
        public ISet<User> GetAllUser();
        #region Groups

        public void EnsureUserIsInGroup(string username, string groupname);
        public void EnsureUserIsNotInGroup(string username, string groupname);
        public bool UserIsInGroup(string username, string groupname);
        public bool GroupExists(string groupname);
        public void EnsureGroupExists(string groupUser);
        public void EnsureGroupDoesNotExist(string groupname);
        public ISet<string> GetGroupsOfUser(string username);
        #endregion
    }
}
