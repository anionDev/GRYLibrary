using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.KeyCloak
{
    /// <summary>
    /// This is a transient keycloak-service for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement any features to increase security.
    /// </remarks>
    public class TransientKeyCloakService : IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
        private readonly ITimeService _TimeService;
        private readonly IDictionary<string/*username*/, UserBackendInformation> _Users;
        private readonly IDictionary<string/*groupname*/, UserGroup> _Groups;
        public TransientKeyCloakService(IKeyCloakServiceSettings settings, ITimeService timeService)
        {
            this.Settings = settings;
            this._TimeService = timeService;
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
            _Users[username].AccessToken.Clear();
        }
    }
}
