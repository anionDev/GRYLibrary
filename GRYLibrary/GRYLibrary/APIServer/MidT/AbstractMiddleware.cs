using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT
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
        public bool EndPointAvailable(HttpContext context)
        {
            return context.GetEndpoint() != null;
        }
        public AuthorizeAttribute GetAuthorizeAttribute(HttpContext context)
        {
            Endpoint endPoint = context.GetEndpoint() ?? throw new BadRequestException((int)System.Net.HttpStatusCode.NotFound, "Not found");
            EndpointMetadataCollection metaData = endPoint?.Metadata;
            ControllerActionDescriptor controllerActionDescriptor = metaData?.GetMetadata<ControllerActionDescriptor>();
            System.Reflection.MethodInfo methodInfo = controllerActionDescriptor?.MethodInfo;
            AuthorizeAttribute authorizeAttribute = methodInfo?.GetCustomAttributes(false).OfType<AuthorizeAttribute>().FirstOrDefault();
            return authorizeAttribute;
        }
    }
}