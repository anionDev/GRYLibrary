namespace GRYLibrary.Core.APIServer.Services.KC
{
    /// <summary>
    /// Represents a service which is able to communicate with a keycloak-server.
    /// </summary>
    public interface IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
        bool UserIsInGroup(string name, string group);
    }
}
