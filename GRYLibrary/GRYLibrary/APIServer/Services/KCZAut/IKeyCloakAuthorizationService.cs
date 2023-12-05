using GRYLibrary.Core.APIServer.Services.Interfaces;

namespace GRYLibrary.Core.APIServer.Services.KCZAut
{
    /// <summary>
    /// Represents a <see cref="IAuthorizationService"/> which authorizes users against a keycloak-server due to a role-based authorization-check.
    /// </summary>
    public interface IKeyCloakAuthorizationService : IUserAuthorizationService
    {
    }
}
