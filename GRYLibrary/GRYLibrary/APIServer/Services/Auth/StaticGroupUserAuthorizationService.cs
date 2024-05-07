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
    public class StaticGroupUserAuthorizationService<UserType> : RoleBasedAuthorizationService
        where UserType : User
    {
        private readonly IAuthenticationService<UserType> _AuthenticationService;

        public StaticGroupUserAuthorizationService(IAuthenticationService<UserType> authenticationService)
        {
            this._AuthenticationService = authenticationService;
        }

        public override bool IsAuthorized(string user, string action, ISet<string> authorizedGroups)
        {
            ISet<string> groupsOfUser = this._AuthenticationService.GetRolesOfUser(user);
            return groupsOfUser.Intersect(authorizedGroups).Any();
        }
    }
}
