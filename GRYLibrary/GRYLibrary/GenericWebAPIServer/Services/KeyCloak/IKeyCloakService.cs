using Keycloak.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.Services.KeyCloak
{
    public interface IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
        public KeycloakClient GetKeycloakClient();
    }
}
