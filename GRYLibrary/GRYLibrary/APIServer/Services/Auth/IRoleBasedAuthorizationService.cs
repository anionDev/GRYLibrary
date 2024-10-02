using GRYLibrary.Core.APIServer.Services.Interfaces;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    public interface IRoleBasedAuthorizationService : IUserAuthorizationService
    {
        public bool IsAuthorized(string userId, ISet<string> authorizedGroups);
    }
}
