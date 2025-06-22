using GRYLibrary.Core.APIServer.Services.Interfaces;

namespace GRYLibrary.Core.APIServer.Services.OpenIDConnectAuth
{
    /// <summary>
    /// Represents a <see cref="IAuthenticationService"/> which authenticates users against a keycloak-server.
    /// </summary>
    public interface IOpenIDConnectAuthenticationService : IAuthenticationService
    {
    }
}
