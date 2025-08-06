using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;

namespace GRYLibrary.Core.APIServer.Services.OpenIDConnect
{
    /// <summary>
    /// Represents a service which is able to communicate with a keycloak-server.
    /// </summary>
    public interface IOpenIDConnectService : IAuthenticationService
    {
        public IOpenIDConnectServiceSettings Settings { get; }
        bool UserIsInGroup(string name, string group);
    }
    public interface IOpenIDConnectService<UserType> : IAuthenticationService
        where UserType : User
    {
        public IOpenIDConnectServiceSettings Settings { get; }
        bool UserIsInGroup(string name, string group);
    }
   
}
