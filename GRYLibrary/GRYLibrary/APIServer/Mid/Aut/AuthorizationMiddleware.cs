using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Aut
{
    public abstract class AuthorizationMiddleware :AbstractMiddleware
    {
        private readonly IAuthorizationConfiguration _AuthorizationConfiguration;
        protected AuthorizationMiddleware(RequestDelegate next,IAuthorizationConfiguration authorizationConfiguration) : base(next)
        {
            this._AuthorizationConfiguration = authorizationConfiguration;
        }
        public virtual bool AuthorizationIsRequired(HttpContext context)
        {
            return true;
        }
        public abstract bool IsAuthorized(HttpContext context);
        public override Task Invoke(HttpContext context)
        {
            if(this.IsAuthorizedInternal(context))
            {
                return this._Next(context);
            }
            else
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
        }
        public virtual bool IsAuthorizedInternal(HttpContext context)
        {
            if(this.AuthorizationIsRequired(context))
            {
                if(this.IsAuthorized(context))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
