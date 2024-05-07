using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    public abstract class RoleBasedAuthorizationService : IRoleBasedAuthorizationService
    {
        public abstract bool IsAuthorized(string user, string action, ISet<string> authorizedGroups);
    }
}
