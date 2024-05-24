using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    /// <summary>
    /// This is a transient <see cref="IUserAuthorizationService"/> for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement any features to increase security.
    /// </remarks>
    public class StaticRoleBasedUserAuthorizationService<UserType> : IRoleBasedAuthorizationService
        where UserType : User
    {
        private readonly IAuthenticationService<UserType> _AuthenticationService;
        public StaticRoleBasedUserAuthorizationService(IAuthenticationService<UserType> authenticationService)
        {
            this._AuthenticationService = authenticationService;
        }

        public bool IsAuthorized(string userId, ISet<string> authorizedGroups)
        {
            ISet<string> groupsOfUser = this._AuthenticationService.GetRolesOfUser(userId);
            return groupsOfUser.Intersect(authorizedGroups).Any();
        }
    }
}
