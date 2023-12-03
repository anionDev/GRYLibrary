using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Mid.Auth.Def;
using GRYLibrary.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services
{
    /// <summary>
    /// This is a transient keycloak-service for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement any features to increase security.
    /// </remarks>
    public class TransientAuthenticationService : IAuthenticationService
    {
        private readonly IDefaultAuthenticationConfiguration _AuthenticationService;
        private readonly ITimeService _TimeService;
        private readonly IDictionary<string/*username*/, UserBackendInformation> _Users;
        private readonly IDictionary<string/*groupname*/, UserGroup> _Groups;
        public TransientAuthenticationService(ITimeService timeService, IDefaultAuthenticationConfiguration defaultAuthenticationConfiguration)
        {
            this._TimeService = timeService;
            _AuthenticationService=defaultAuthenticationConfiguration;
            this._Users = new Dictionary<string, UserBackendInformation>();
            this._Groups = new Dictionary<string, UserGroup>();
        }
        public bool AccessTokenIsValid(string username, string accessToken)
        {
            try
            {
                UserBackendInformation user = this.GetUserByName(username);
                AccessToken token = user.AccessToken[accessToken];
                return this._TimeService.GetCurrentTime() < token.ExpiredMoment;
            }
            catch
            {
                return false;
            }
        }

        private UserBackendInformation GetUserByName(string username)
        {
            if (this._Users.TryGetValue(username, out UserBackendInformation value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"No user found with username {username}.");
            }
        }


        public AccessToken Login(string username, string password)
        {
            UserBackendInformation user = this.GetUserByName(username);
            if (password == user.Password)
            {
                AccessToken newAccessToken = new AccessToken();
                newAccessToken.Value = Guid.NewGuid().ToString();
                newAccessToken.ExpiredMoment = this._TimeService.GetCurrentTime().AddDays(1);//this time should be moved to IKeyCloakServiceSettings if it is implementable in the real keycloack-service too.
                user.AccessToken[newAccessToken.Value] = newAccessToken;
                return newAccessToken;
            }
            else
            {
                throw new UserFormattedException("Invalid password.");
            }
        }

        public void Register(string username, string password)
        {
            UserBackendInformation userBackendInformation = new UserBackendInformation()
            {
                User = new User()
                {
                    Id = Guid.NewGuid(),
                    Name = username,
                },
                Password = password,
            };
            if (this._Users.ContainsKey(userBackendInformation.User.Name))
            {
                throw new BadUserContentException("User with already exists.");
            }
            else
            {
                this._Users.Add(userBackendInformation.User.Name, userBackendInformation);
            }
        }

        public void Logout(string username)
        {
            this._Users[username].AccessToken.Clear();
        }

        public void EnsureUserIsInGroup(string username, string groupname)
        {
            UserBackendInformation user = this.GetUserByName(username);
            if (!this._Groups[groupname].User.Contains(user.User.Id))
            {
                this._Groups[groupname].User.Add(user.User.Id);
            }
        }

        public void EnsureUserIsNotInGroup(string username, string groupname)
        {
            UserBackendInformation user = this.GetUserByName(username);
            if (this._Groups[groupname].User.Contains(user.User.Id))
            {
                this._Groups[groupname].User.Remove(user.User.Id);
            }
        }

        public bool UserIsInGroup(string username, string groupname)
        {
            return this._Groups[groupname].User.Contains(this.GetUserByName(username).User.Id);
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

        public virtual void OnStart()
        {
        }

        public void RemoveUser(string username)
        {
            this._Users.Remove(username);
        }
    }
}
