using Keycloak.Net;
using Keycloak.Net.Models.Users;
using System.Collections.Generic;
using KeycloakUser = Keycloak.Net.Models.Users.User;
using System.Threading.Tasks;
using System;

namespace GRYLibrary.Core.APIServer.Services.KeyCloak
{
    public abstract class KeyCloakService : IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
        public KeycloakClient KeycloakClient { get; private set; }
        public KeyCloakService(IKeyCloakServiceSettings settings)
        {
            this.Settings = settings;
            this.Initialize();
        }

        private void Initialize()
        {
            this.KeycloakClient = new KeycloakClient(this.Settings.URL, this.Settings.User, this.Settings.Password);
        }

        public KeycloakClient GetKeycloakClient()
        {
            return this.KeycloakClient;
        }

        public virtual bool AccessTokenIsValid(string actionName, string accessToken, string username)
        {
            throw new NotImplementedException();
        }

        public virtual void Register(string username, string password, bool enabled)
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
            user.Enabled = enabled;
            Task<string> task = service.CreateAndRetrieveUserIdAsync(this.Settings.Realm, user);
            task.Wait();
        }

        public virtual string Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}
