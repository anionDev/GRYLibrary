using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Mid.AutS
{
    /// <summary>
    /// Represents an <see cref="AuthorizationMiddleware"/> which implements action-based authorizaton-checks using a <see cref="IActionBasedAuthorizationService"/>.
    /// </summary>
    public class AutSAMiddleware : AuthorizationMiddleware
    {
        private readonly IActionBasedAuthorizationService _AuthorizationService;
        private readonly IAuthenticationService _AuthenticationService;
        private readonly ICredentialsProvider _CredentialsProvider;
        public AutSAMiddleware(RequestDelegate next, IActionBasedAuthorizationService authorizationService, IAuthenticationService authenticationService, ICredentialsProvider credentialsProvider) : base(next)
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
            ActionAttribute actionAttribute = this.GetActionAttribute(context);
            GUtilities.AssertCondition(this._CredentialsProvider.ContainsCredentials(context));
            string accessToken = this._CredentialsProvider.ExtractSecret(context);
            User user = this._AuthenticationService.GetUserByAccessToken(accessToken);
            return this._AuthorizationService.IsAuthorized(user.Id, actionAttribute.Action);
        }
    }
}
