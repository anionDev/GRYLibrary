using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
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
        public bool IsAuthorized(string action, string secret)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(string action, string user, string secret)
        {
            throw new NotImplementedException();
        }

        public void EnsureUserIsInGroup(string username, string groupname)
        {
            UserBackendInformation user = this.GetUserByName(username);
            if (!this._Groups[groupname].UserIds.Contains(user.User.Id))
            {
                this._Groups[groupname].UserIds.Add(user.User.Id);
            }
        }

        private UserBackendInformation GetUserByName(string username)
        {
            return this._AuthenticationService.GetUserByName(username);
        }

        public void EnsureUserIsNotInGroup(string username, string groupname)
        {
            UserBackendInformation user = this.GetUserByName(username);
            if (this._Groups[groupname].UserIds.Contains(user.User.Id))
            {
                this._Groups[groupname].UserIds.Remove(user.User.Id);
            }
        }

        public bool UserIsInGroup(string username, string groupname)
        {
            return this._Groups[groupname].UserIds.Contains(this.GetUserByName(username).User.Id);
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

        public void AssertIsAuthorized(string action, string user, string secret)
        {
            if (!this.IsAuthorized(action, user, secret))
            {
                throw new BadRequestException((int)System.Net.HttpStatusCode.Forbidden, "Not authorized.");
            }
        }

        public bool IsAuthorized(HttpContext context, AuthorizeAttribute authorizedAttribute)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = this._AuthenticationService.GetUserByName(context.User.Identity.Name);
                var authorizedGroups = authorizedAttribute.Groups;

                foreach (var authorizedGroup in authorizedGroups)
                {
                    if (this._Groups.ContainsKey(authorizedGroup))
                    {
                        var group = this._Groups[authorizedGroup];
                        if (group.UserIds.Contains(user.User.Id))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
