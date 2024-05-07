using GRYLibrary.Core.APIServer.Services.Interfaces;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    public interface IRoleBasedAuthorizationService : IUserAuthorizationService
    {
        public bool IsAuthorized(string user, string action, ISet<string> authorizedGroups);
    }
}
