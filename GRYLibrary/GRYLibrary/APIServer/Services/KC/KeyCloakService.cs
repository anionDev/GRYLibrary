using Keycloak.Net;
using Keycloak.Net.Models.Users;
using System.Collections.Generic;
using KeycloakUser = Keycloak.Net.Models.Users.User;
using System.Threading.Tasks;
using System;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using Microsoft.AspNetCore.Http;
using GRYLibrary.Core.APIServer.Utilities;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Services.KeyCloak
{
    public abstract class KeyCloakService : ExternalService, IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
        public KeycloakClient KeycloakClient { get; private set; }
        public KeyCloakService(IKeyCloakServiceSettings settings, IGeneralLogger logger) : base(nameof(KeyCloakService), logger)
        {
            this.Settings = settings;
            this.Initialize();
        }

        private void Initialize()
        {
            this.KeycloakClient = new KeycloakClient(this.Settings.URL, this.Settings.User, this.Settings.Password/*TODO use clientSecret instead*/);
            base.TryConnect(out Exception _);
        }

        protected KeycloakClient GetKeycloakClient()
        {
            return this.KeycloakClient;
        }

        public virtual bool AccessTokenIsValid(string username, string accessToken)
        {
            return this.EnsureServiceIsConnected<bool>(() =>
            {
                throw new NotImplementedException();
            });
        }

        public virtual void Register(string username, string password)
        {
            this.EnsureServiceIsConnected(() =>
            {
                KeycloakClient service = this.GetKeycloakClient();
                KeycloakUser user = new KeycloakUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = username
                };
                user.FirstName = string.Empty;
                user.LastName = username;
                user.Groups = new List<string>();
                user.ClientRoles = new Dictionary<string, object>();
                user.Credentials = new List<Credentials>() { };
                //user.Email = $"{username}@{Realm}";
                user.Enabled = true;
                Task<string> task = service.CreateAndRetrieveUserIdAsync(this.Settings.Realm, user);
                task.Wait();
            });
        }

        public AccessToken Login(string username, string password)
        {
            return this.EnsureServiceIsConnected<AccessToken>(() =>
            {
                throw new NotImplementedException();
            });
        }

        public void Logout(string name)
        {
            throw new NotImplementedException();
        }

        public void EnsureUserIsInGroup(string user, string group)
        {
            throw new NotImplementedException();
        }

        public void EnsureUserIsNotInGroup(string user, string group)
        {
            throw new NotImplementedException();
        }

        public bool UserIsInGroup(string user, string group)
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            throw new NotImplementedException();
        }

        public bool GroupExists(string groupname)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(string username)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthenticated(HttpContext context, AuthorizeAttribute authorizeAttribute)
        {
            throw new NotImplementedException();
        }

        public bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
