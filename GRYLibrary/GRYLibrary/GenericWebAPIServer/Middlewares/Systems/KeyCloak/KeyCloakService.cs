using Keycloak.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakService :IKeyCloakService
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
            this.KeycloakClient = new KeycloakClient(this.Settings.URL, this.Settings.User, this.Settings.Password, new KeycloakOptions(adminClientId: "admin"));
        }

        public KeycloakClient GetKeycloakClient()
        {
            return this.KeycloakClient;
        }
    }
}
