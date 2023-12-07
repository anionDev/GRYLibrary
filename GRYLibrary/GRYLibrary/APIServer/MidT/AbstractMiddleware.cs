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
        public AuthorizeAttribute GetAuthorizeAttribute(HttpContext context)
        {
            Endpoint endPoint = context.GetEndpoint();
            if(endPoint == null)
            {
                throw new BadRequestException(System.Net.HttpStatusCode.NotFound,"Not found");
            }
            EndpointMetadataCollection metaData = endPoint!=null? endPoint.Metadata:null;
            ControllerActionDescriptor controllerActionDescriptor = metaData != null ? metaData.GetMetadata<ControllerActionDescriptor>() : null;
            System.Reflection.MethodInfo methodInfo = controllerActionDescriptor != null ? controllerActionDescriptor.MethodInfo : null;
            AuthorizeAttribute authorizeAttribute = methodInfo != null? methodInfo.GetCustomAttributes(false).OfType<AuthorizeAttribute>().FirstOrDefault():null;
            return authorizeAttribute;
        }
    }
}