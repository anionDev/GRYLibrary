using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public abstract class AuthorizationMiddleware : AbstractMiddleware
    {
        protected AuthorizationMiddleware(RequestDelegate next) : base(next)
        {
        }
        public abstract bool AuthorizationIsRequired(HttpContext context);
        protected abstract bool IsAuthorized(HttpContext context);
        public override Task Invoke(HttpContext context)
        {
            if (this.AuthorizationIsRequired(context) && !this.IsAuthorizedInternal(context))
            {
                throw new BadRequestException(StatusCodes.Status403Forbidden);
            }
            else
            {
                return this._Next(context);
            }
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
