using Keycloak.Net;
using System;

namespace GRYLibrary.Core.APIServer.Services.KeyCloak
{
    public class TransientKeyCloakService : IKeyCloakService
    {
        
        public IKeyCloakServiceSettings Settings { get; }
        public TransientKeyCloakService(IKeyCloakServiceSettings settings)
        {
            this.Settings = settings;
        }
        public bool AccessTokenIsValid(string actionName, string accessToken, string username)
        {
            throw new NotImplementedException();
        }

        public KeycloakClient GetKeycloakClient()
        {
            throw new NotImplementedException();
        }

        public string Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Register(string username, string password, bool enabled)
        {
            throw new NotImplementedException();
        }
    }
}
