using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    /// <summary>
    /// This is a transient <see cref="IUserAuthorizationService"/> for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement any features to increase security.
    /// </remarks>
    public class TransientUserAuthorizationService : IUserAuthorizationService
    {
        private readonly IAuthenticationService _AuthenticationService;
        private readonly IDictionary<string/*groupname*/, UserGroup> _Groups;

        public TransientUserAuthorizationService(IAuthenticationService authenticationService)
        {
            this._Groups = new Dictionary<string, UserGroup>();
            this._AuthenticationService = authenticationService;
        }

        public void EnsureUserIsInGroup(string username, string groupname)
        {
           string userId = this._AuthenticationService.GetIdOfUser(username);
            this._Groups[groupname].UserIds.Add(userId);
        }

        public void EnsureUserIsNotInGroup(string username, string groupname)
        {
            string userId = this._AuthenticationService.GetIdOfUser(username);
            this._Groups[groupname].UserIds.Remove(userId);
        }

        public bool UserIsInGroup(string username, string groupname)
        {
            string userId = this._AuthenticationService.GetIdOfUser(username);
            return this._Groups[groupname].UserIds.Contains(userId);
        }

        public void EnsureGroupExists(string groupname)
        {
            if (!this._Groups.ContainsKey(groupname))
            {
                UserGroup group = new UserGroup();
                group.Name = groupname;
                this._Groups[groupname] = group;
            }
        }

        public void EnsureGroupDoesNotExist(string groupname)
        {
            if (this._Groups.ContainsKey(groupname))
            {
                this._Groups.Remove(groupname);
            }
        }
        public bool GroupExists(string groupname)
        {
            return this._Groups.ContainsKey(groupname);
        }

        public bool IsAuthorized(string user, string action)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetGroupsOfUser(string username)
        {
            throw new NotImplementedException();
        }

    }
}
