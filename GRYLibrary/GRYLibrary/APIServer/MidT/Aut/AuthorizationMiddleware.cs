using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public abstract class AuthorizationMiddleware : AbstractMiddleware
    {
        protected AuthorizationMiddleware(RequestDelegate next) : base(next)
        {
        }
        public virtual bool AuthorizationIsRequired(HttpContext context)
        {
            AuthorizeAttribute authorizeAttribute = this.GetAuthorizeAttribute(context);
            return authorizeAttribute != null;
        }
        protected abstract bool IsAuthorized(HttpContext context);
        public override Task Invoke(HttpContext context)
        {
            if (this.AuthorizationIsRequired(context) && !this.IsAuthorizedInternal(context))
            {
                return this.ReturnUnauthorizedResult(context);
            }
            else
            {
                return this._Next(context);
            }
        }
        public virtual Task ReturnUnauthorizedResult(HttpContext context)
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        public virtual bool IsAuthorizedInternal(HttpContext context)
        {
            if (this.IsAuthorized(context))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
