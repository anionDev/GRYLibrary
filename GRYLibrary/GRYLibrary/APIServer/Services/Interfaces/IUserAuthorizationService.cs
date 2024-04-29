using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IUserAuthorizationService : IAuthorizationService
    {
        public bool IsAuthorized(string user, string action);
        public bool IsAuthorized(string user, string action, ISet<string> authorizedGroups);
    }
}
