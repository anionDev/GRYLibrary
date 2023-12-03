using GRYLibrary.Core.APIServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Aut.Auth
{
    public  class AuthAuthorizationMiddleware : AuthorizationMiddleware
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IAuthorizationConfiguration _AuthorizationConfiguration;
#pragma warning restore IDE0052 // Remove unread private members
        private readonly IAuthenticationService _AuthenticationService;
        protected AuthAuthorizationMiddleware(RequestDelegate next, IAuthorizationConfiguration authorizationConfiguration, IAuthenticationService authenticationService) : base(next, authorizationConfiguration)
        {
            this._AuthorizationConfiguration = authorizationConfiguration;
            this._AuthenticationService = authenticationService;
        }
        public override bool AuthorizationIsRequired(HttpContext context)
        {
            AuthorizeAttribute authorizeAttribute = GetAuthorizeAttribute(context);
            return authorizeAttribute.Groups.Any();
        }
        public override bool IsAuthorized(HttpContext context)
        {
            AuthorizeAttribute authorizedAttribute = this.GetAuthorizeAttribute(context);
            if (authorizedAttribute == null)
            {
                return true;
            }
            else
            {
                System.Collections.Generic.ISet<string> groups = authorizedAttribute.Groups;
                foreach (string group in groups)
                {
                    if (this._AuthenticationService.UserIsInGroup(context.User.Identity.Name, group))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public override Task Invoke(HttpContext context)
        {
            if (this.IsAuthorizedInternal(context))
            {
                return this._Next(context);
            }
            else
            {
                return this.ReturnUnauthenticatedResult(context);
            }
        }
    }
}
