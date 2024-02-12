using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Mid.Auth
{
    /// <summary>
    /// Represents an <see cref="AuthorizationMiddleware"/> which implements authorizaton-checks based on group-memberships defined by <see cref="IUserAuthorizationService"/>.
    /// </summary>
    public class AutSMiddleware : AuthorizationMiddleware
    {
        private readonly IUserAuthorizationService _AuthorizationService;
        private readonly IAuthenticationService _AuthenticationService;
        private readonly IHTTPCredentialsProvider _CredentialsProvider;
        public AutSMiddleware(RequestDelegate next, IUserAuthorizationService authorizationService, IAuthenticationService authenticationService, IHTTPCredentialsProvider credentialsProvider) : base(next)
        {
            this._AuthorizationService = authorizationService;
            this._AuthenticationService = authenticationService;
            this._CredentialsProvider = credentialsProvider;
        }
        public override bool AuthorizationIsRequired(HttpContext context)
        {
            AuthorizeAttribute authorizeAttribute = this.GetAuthorizeAttribute(context);
            if (authorizeAttribute == null)
            {
                return false;
            }
            else
            {
                return authorizeAttribute.Groups.Any();
            }
        }
        protected override bool IsAuthorized(HttpContext context)
        {
            AuthorizeAttribute authorizedAttribute = this.GetAuthorizeAttribute(context);
            System.Collections.Generic.ISet<string> authorizedGroups =authorizedAttribute.Groups;
            GUtilities.AssertCondition(this._CredentialsProvider.ContainsCredentials(context));
            string accessToken = this._CredentialsProvider.ExtractSecret(context);
            string username = this._AuthenticationService.GetUserName(accessToken);
            System.Collections.Generic.ISet<string> groupsOfUser = this._AuthorizationService.GetGroupsOfUser(username);
            return authorizedGroups.Intersect(groupsOfUser).Any();
        }
    }
}
