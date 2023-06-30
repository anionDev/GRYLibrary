using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.CommonRoutes
{
    [ApiController]
    [Route($"{ServerConfiguration.APIRoutePrefix}/Other/Resources/Information")]
    public class CommonRoutesController :Controller
    {
        private readonly IApplicationConstants _Configuration;
        private readonly ICommonRoutesInformation _CommonRoutesInformation;
        private readonly IEnumerable<EndpointDataSource> _EndpointSources;
        public CommonRoutesController(IApplicationConstants configuration, ICommonRoutesInformation commonRoutesInformation, IEnumerable<EndpointDataSource> endpointSources)
        {
            this._Configuration = configuration;
            _CommonRoutesInformation = commonRoutesInformation;
            _EndpointSources = endpointSources;
        }

        [HttpGet]
        [Route(nameof(TermsOfService))]
        public IActionResult TermsOfService()
        {
            return this.Redirect(_CommonRoutesInformation.TermsOfServiceLink);
        }

        [HttpGet]
        [Route(nameof(Contact))]
        public IActionResult Contact()
        {
            return this.Redirect(_CommonRoutesInformation.ContactLink);
        }

        [HttpGet]
        [Route(nameof(License))]
        public IActionResult License()
        {
            return this.Redirect(_CommonRoutesInformation.LicenseLink);
        }

        [HttpGet("ShowAllEndpoints")]
        public IActionResult ShowAllEndpoints()
        {
            var endpoints = _EndpointSources.SelectMany(es => es.Endpoints).OfType<RouteEndpoint>();
            var output = endpoints.Select(endPoint =>
            {
                var controller = endPoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
                var action = controller != null ? $"{controller.ControllerName}.{controller.ActionName}" : null;
                var controllerMethod = controller != null ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}" : null;
                return new
                {
                    Method = endPoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                    Route = $"/{endPoint.RoutePattern.RawText.TrimStart('/')}",
                    Action = action,
                    ControllerMethod = controllerMethod
                };
            }
            );
            return Json(output);
        }
    }
}
