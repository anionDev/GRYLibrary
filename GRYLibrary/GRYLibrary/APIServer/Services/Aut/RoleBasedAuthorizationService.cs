﻿using GRYLibrary.Core.APIServer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Services.Aut
{
    public class RoleBasedAuthorizationService : IUserAuthorizationService
    {
        private readonly IAuthenticationService _AuthenticationService;
        private readonly IRoleBasedAuthorizationPersistence _RoleBasedAuthorizationPersistence;
        public RoleBasedAuthorizationService(IAuthenticationService authenticationService, IRoleBasedAuthorizationPersistence roleBasedAuthorizationPersistence)
        {
            this._AuthenticationService = authenticationService;
            this._RoleBasedAuthorizationPersistence =roleBasedAuthorizationPersistence;
        }

        public bool IsAuthorized(string user, string action, ISet<string> authorizedGroups)
        {
            ISet<string> groupsOfUser = this._AuthenticationService.GetGroupsOfUser(user);
            return authorizedGroups.Intersect(groupsOfUser).Any();
        }

        public bool IsAuthorized(string user, string action)
        {
            ISet<string> groupsOfUser = this._AuthenticationService.GetGroupsOfUser(user);
            return _RoleBasedAuthorizationPersistence.GetAuhorizedGroupsForAction(action).Intersect(groupsOfUser).Any();
        }
    }
}
