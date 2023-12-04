using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Mid.Auth
{
    /// <summary>
    /// Represents an <see cref="AuthorizationMiddleware"/> which implements authorizaton-checks using group-memberships defined by <see cref="IAuthorizationService"/>.
    /// </summary>
    public class AutSMiddleware : AuthorizationMiddleware
    {
        private readonly IAuthorizationService _AuthorizationService;
        protected AutSMiddleware(RequestDelegate next, IAuthorizationService authorizationService) : base(next)
        {
            this._AuthorizationService = authorizationService;
        }
        public override bool AuthorizationIsRequired(HttpContext context)
        {
            AuthorizeAttribute authorizeAttribute = this.GetAuthorizeAttribute(context);
            return authorizeAttribute.Groups.Any();
        }
        protected override bool IsAuthorized(HttpContext context)
        {
            AuthorizeAttribute authorizedAttribute = this.GetAuthorizeAttribute(context);
            if (authorizedAttribute == null)
            {
                return true;
            }
            else
            {
                return _AuthorizationService.IsAuthorized("context", "authorizedAttribute");//TODO
            }
        }
    }
}
