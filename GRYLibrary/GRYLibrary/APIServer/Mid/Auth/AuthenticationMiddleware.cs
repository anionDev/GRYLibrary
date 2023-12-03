using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Auth
{
    public abstract class AuthenticationMiddleware : AbstractMiddleware
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IAuthenticationConfiguration _Configuration;
#pragma warning restore IDE0052 // Remove unread private members
        protected AuthenticationMiddleware(RequestDelegate next, IAuthenticationConfiguration configuration) : base(next)
        {
            this._Configuration = configuration;
        }
        public virtual bool AuthenticatedIsRequired(HttpContext context)
        {
            Endpoint endPoint = context.GetEndpoint();
            EndpointMetadataCollection metaData = endPoint.Metadata;
            ControllerActionDescriptor controllerActionDescriptor = metaData.GetMetadata<ControllerActionDescriptor>();
            System.Reflection.MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
            AuthorizeAttribute authorizeAttribute = methodInfo.GetCustomAttributes(false).OfType<AuthorizeAttribute>().FirstOrDefault();
            return authorizeAttribute != null;
        }
        public abstract bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal);
        public override Task Invoke(HttpContext context)
        {
            if (!this.IsAuthenticatedInternal(context) && this.AuthenticatedIsRequired(context))
            {
                return this.ReturnForbidResult(context);
            }
            else
            {
                return this._Next(context);
            }
        }
        public virtual Task ReturnForbidResult(HttpContext context)
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
