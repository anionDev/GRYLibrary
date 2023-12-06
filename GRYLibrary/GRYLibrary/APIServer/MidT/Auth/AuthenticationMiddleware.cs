using GRYLibrary.Core.APIServer.Utilities;
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
            if (!this.IsAuthenticatedInternal(context)&&this.AuthenticationIsRequired(context) )
            {
                return this.ReturnAuthenticationRequiredResult(context);
            }
            else
            {
                return this._Next(context);
            }
        }
        public virtual Task ReturnAuthenticationRequiredResult(HttpContext context)
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
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
