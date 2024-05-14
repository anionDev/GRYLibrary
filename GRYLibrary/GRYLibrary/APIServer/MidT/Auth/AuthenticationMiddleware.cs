using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public abstract class AuthenticationMiddleware : AbstractMiddleware
    {
        protected AuthenticationMiddleware(RequestDelegate next) : base(next)
        {
        }
        public virtual bool AuthenticationIsRequired(HttpContext context)
        {
            AuthorizeAttribute authorizeAttribute = this.GetAuthorizeAttribute(context);
            return authorizeAttribute != null;
        }
        public abstract bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal);
        public override Task Invoke(HttpContext context)
        {
            if (!this.IsAuthenticatedInternal(context) && this.AuthenticationIsRequired(context))
            {
                 throw new BadRequestException(StatusCodes.Status401Unauthorized);
            }
            else
            {
                return this._Next(context);
            }
        }

        public virtual bool IsAuthenticatedInternal(HttpContext context)
        {
            if (this.TryGetAuthentication(context, out ClaimsPrincipal principal))
            {
                context.User = principal;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
