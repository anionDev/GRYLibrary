using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.CommonRoutes
{
    [ApiController]
    [Route($"{ServerConfiguration.APIRoutePrefix}/Other/Resources/Information")]
    public class CommonRoutesController : Controller
    {
        private readonly IApplicationConstants _Configuration;
        private readonly ICommonRoutesInformation _CommonRoutesInformation;
        private readonly IEnumerable<EndpointDataSource> _EndpointSources;
        public CommonRoutesController(IApplicationConstants configuration, ICommonRoutesInformation commonRoutesInformation, IEnumerable<EndpointDataSource> endpointSources)
        {
            this._Configuration = configuration;
            this._CommonRoutesInformation = commonRoutesInformation;
            this._EndpointSources = endpointSources;
        }

        [HttpGet]
        [Route(nameof(TermsOfService))]
        public IActionResult TermsOfService()
        {
            return this.Redirect(this._CommonRoutesInformation.TermsOfServiceLink);
        }

        [HttpGet]
        [Route(nameof(Contact))]
        public IActionResult Contact()
        {
            return this.Redirect(this._CommonRoutesInformation.ContactLink);
        }

        [HttpGet]
        [Route(nameof(License))]
        public IActionResult License()
        {
            return this.Redirect(this._CommonRoutesInformation.LicenseLink);
        }

        [HttpGet("ShowAllEndpoints")]
        public IActionResult ShowAllEndpoints()
        {
            IEnumerable<RouteEndpoint> endpoints = this._EndpointSources.SelectMany(es => es.Endpoints).OfType<RouteEndpoint>();
            var output = endpoints.Select(endPoint =>
            {
                ControllerActionDescriptor controller = endPoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
                string action = controller != null ? $"{controller.ControllerName}.{controller.ActionName}" : null;
                string controllerMethod = controller != null ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}" : null;
                return new
                {
                    Method = endPoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                    Route = $"/{endPoint.RoutePattern.RawText.TrimStart('/')}",
                    Action = action,
                    ControllerMethod = controllerMethod
                };
            }
            );
            return this.Json(output);
        }
    }
}
