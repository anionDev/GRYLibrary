using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IUserAuthorizationService : IAuthorizationService
    {
        public bool IsAuthorized(string user, string action);
        public void EnsureUserIsInGroup(string username, string groupname);
        public void EnsureUserIsNotInGroup(string username, string groupname);
        public bool UserIsInGroup(string username, string groupname);
        public bool GroupExists(string groupname);
        public void EnsureGroupExists(string groupUser);
        public void EnsureGroupDoesNotExist(string groupname);
        public ISet<string> GetGroupsOfUser(string username);
    }
}
