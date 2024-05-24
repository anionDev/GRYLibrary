using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Mid.Auth
{
    /// <summary>
    /// Represents an <see cref="AuthorizationMiddleware"/> which implements role-based authorizaton-checks using a <see cref="IRoleBasedAuthorizationService"/>.
    /// </summary>
    public class AutSRMiddleware : AuthorizationMiddleware
    {
        private readonly IRoleBasedAuthorizationService _AuthorizationService;
        private readonly IAuthenticationService _AuthenticationService;
        private readonly ICredentialsProvider _CredentialsProvider;
        public AutSRMiddleware(RequestDelegate next, IRoleBasedAuthorizationService authorizationService, IAuthenticationService authenticationService, ICredentialsProvider credentialsProvider) : base(next)
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
            GUtilities.AssertCondition(this._CredentialsProvider.ContainsCredentials(context));
            string accessToken = this._CredentialsProvider.ExtractSecret(context);
            User user = this._AuthenticationService.GetUserByAccessToken(accessToken);
            System.Collections.Generic.ISet<string> authorizedGroups = authorizedAttribute.Groups;
            return this._AuthorizationService.IsAuthorized(user.Id, authorizedGroups);
        }
    }
}
