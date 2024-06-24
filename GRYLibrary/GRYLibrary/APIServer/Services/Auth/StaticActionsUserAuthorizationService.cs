using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    /// <summary>
    /// This is a transient <see cref="IUserAuthorizationService"/> for testing purposes.
    /// </summary>
    public class StaticActionsUserAuthorizationService<UserType> : IActionBasedAuthorizationService
        where UserType : User
    {
        private readonly IAuthenticationService<UserType> _AuthenticationService;
        private readonly IStaticRoleBasedUserAuthorizationServiceConfiguration _StaticGroupUserAuthorizationServiceConfiguration;
        public StaticActionsUserAuthorizationService(IAuthenticationService<UserType> authenticationService, IStaticRoleBasedUserAuthorizationServiceConfiguration staticGroupUserAuthorizationServiceConfiguration)
        {
            this._AuthenticationService = authenticationService;
            this._StaticGroupUserAuthorizationServiceConfiguration = staticGroupUserAuthorizationServiceConfiguration;
        }

        public bool IsAuthorized(string userId, string action)
        {
            ISet<string> groupsOfUser = this._AuthenticationService.GetRolesOfUser(userId);
            return groupsOfUser.Intersect(this._StaticGroupUserAuthorizationServiceConfiguration.AuthorizedGroups[action]).Any();
        }
    }
}
