using GRYLibrary.Core.APIServer.Services.Interfaces;
using System;
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
    public class StaticGroupUserAuthorizationService : IUserAuthorizationService
    {
        private readonly IAuthenticationService _AuthenticationService;

        public StaticGroupUserAuthorizationService(IAuthenticationService authenticationService)
        {
            this._AuthenticationService = authenticationService;
        }

        public bool IsAuthorized(string user, string action)
        {
            throw new NotSupportedException();
        }

        public bool IsAuthorized(string user, string action, ISet<string> authorizedGroups)
        {
            ISet<string> groupsOfUser = this._AuthenticationService.GetGroupsOfUser(user);
            return groupsOfUser.Intersect(authorizedGroups).Any();
        }
    }
}
