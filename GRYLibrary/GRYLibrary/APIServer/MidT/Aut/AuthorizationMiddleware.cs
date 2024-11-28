using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Aut
{
    public abstract class AuthorizationMiddleware : AbstractMiddleware
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IAuthorizationConfiguration _AuthorizationConfiguration;
#pragma warning restore IDE0052 // Remove unread private members
        protected AuthorizationMiddleware(RequestDelegate next, IAuthorizationConfiguration authorizationConfiguration) : base(next)
        {
            this._AuthorizationConfiguration = authorizationConfiguration;
        }
        public bool AuthorizationIsRequired(HttpContext context)
        {
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
