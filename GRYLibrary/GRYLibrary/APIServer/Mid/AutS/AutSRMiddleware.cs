using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.MidT.Aut;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Auth.R;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Linq;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Mid.AutS
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
            if (!(bool)context.Items[AuthenticationMiddleware.IsAuthenticatedInformationName])
            {
                return false;
            }
            if (this.TryGetAuthorizeAttribute(context, out AuthorizeAttribute authorizeAttribute))
            {
                bool result = authorizeAttribute.Groups.Any();
                return result;
            }
            else
            {
                return false;
            }
        }

        protected override bool IsAuthorized(HttpContext context)
        {
            if (this.TryGetAuthorizeAttribute(context, out AuthorizeAttribute authorizedAttribute))
            {
                GUtilities.AssertCondition(this._CredentialsProvider.ContainsCredentials(context));
                string accessToken = this._CredentialsProvider.ExtractSecret(context);
                User user = this._AuthenticationService.GetUserByAccessToken(accessToken);
                System.Collections.Generic.ISet<string> authorizedGroups = authorizedAttribute.Groups;
                bool result = this._AuthorizationService.IsAuthorized(user.GetAllRoles().Select(r => r.Name).ToHashSet(), authorizedGroups);
                return result;
            }
            else
            {
                throw new InternalAlgorithmException();
            }
        }
    }
}
