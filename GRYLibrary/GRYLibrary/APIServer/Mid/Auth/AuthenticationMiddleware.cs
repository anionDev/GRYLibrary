using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Auth
{
    public abstract class AuthenticationMiddleware : AbstractMiddleware
    {
        private readonly IAuthenticationConfiguration _Configuration;
        protected AuthenticationMiddleware(RequestDelegate next, IAuthenticationConfiguration configuration) : base(next)
        {
            this._Configuration = configuration;
        }
        public virtual bool AuthenticatedIsRequired(HttpContext context)
        {
            return true;
        }
        public abstract bool IsAuthenticated(HttpContext context);
        public override Task Invoke(HttpContext context)
        {
            if (this.IsAuthenticatedInternal(context))
            {
                return this._Next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
        }
        public virtual bool IsAuthenticatedInternal(HttpContext context)
        {
            if (this.AuthenticatedIsRequired(context))
            {
                if (this.IsAuthenticated(context))
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
