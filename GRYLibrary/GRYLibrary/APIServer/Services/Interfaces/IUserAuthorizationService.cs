namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IUserAuthorizationService : IAuthorizationService
    {
        public void AssertIsAuthorized(string action, string user, string secret);
        public bool IsAuthorized(string action, string user, string secret);
        public void EnsureUserIsInGroup(string username, string groupname);
        public void EnsureUserIsNotInGroup(string username, string groupname);
        public bool UserIsInGroup(string username, string groupname);
        public bool GroupExists(string groupname);
        void EnsureGroupExists(string groupUser);
    }
}
