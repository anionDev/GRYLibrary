using GRYLibrary.Core.APIServer.Mid.Aut;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid
{
    /// <summary>
    /// Represents a base-class for middlewares in the context of .NET WebAPI-projects.
    /// </summary>
    public abstract class AbstractMiddleware
    {
        protected readonly RequestDelegate _Next;
        /// <summary>
        /// Creates a new middleware.
        /// </summary>
        public AbstractMiddleware(RequestDelegate next)
        {
            this._Next = next;
        }
        public abstract Task Invoke(HttpContext context);
        public AuthorizeAttribute GetAuthorizeAttribute(HttpContext context)
        {
            Endpoint endPoint = context.GetEndpoint();
            EndpointMetadataCollection metaData = endPoint.Metadata;
            ControllerActionDescriptor controllerActionDescriptor = metaData.GetMetadata<ControllerActionDescriptor>();
            System.Reflection.MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
            AuthorizeAttribute authorizeAttribute = methodInfo.GetCustomAttributes(false).OfType<AuthorizeAttribute>().First();
            return authorizeAttribute;
        }
    }
}