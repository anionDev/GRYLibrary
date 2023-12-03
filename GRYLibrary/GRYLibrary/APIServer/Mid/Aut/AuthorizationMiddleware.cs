using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Aut
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
        public virtual bool AuthorizationIsRequired(HttpContext context)
        {
            Endpoint endPoint = context.GetEndpoint();
            EndpointMetadataCollection metaData = endPoint.Metadata;
            ControllerActionDescriptor controllerActionDescriptor = metaData.GetMetadata<ControllerActionDescriptor>();
            System.Reflection.MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
            AuthorizeAttribute authorizeAttribute = methodInfo.GetCustomAttributes(false).OfType<AuthorizeAttribute>().First();
            return string.IsNullOrEmpty(authorizeAttribute.Groups);
        }
        public abstract bool IsAuthorized(HttpContext context);
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
        public virtual Task ReturnUnauthenticatedResult(HttpContext context)
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        public virtual bool IsAuthorizedInternal(HttpContext context)
        {
            if (this.AuthorizationIsRequired(context))
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
            else
            {
                return true;
            }
        }
    }
}
