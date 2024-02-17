using Microsoft.AspNetCore.Mvc;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Diagnostics;

namespace GRYLibrary.Core.APIServer.MaintenanceRoutes
{
    [ApiController]
    [Route($"{ServerConfiguration.APIRoutePrefix}/Other/Maintenance")]
    public class MaintenanceRoutesController : Controller
    {
        private readonly IApplicationConstants _Configuration;
        private readonly IMaintenanceRoutesInformation _MaintenanceRoutesInformation;
        private readonly IEnumerable<EndpointDataSource> _EndpointSources;
        public MaintenanceRoutesController(IApplicationConstants configuration, IMaintenanceRoutesInformation maintenanceRoutesInformation, IEnumerable<EndpointDataSource> endpointSources)
        {
            this._Configuration = configuration;
            this._MaintenanceRoutesInformation = maintenanceRoutesInformation;
            this._EndpointSources = endpointSources;
        }

        [HttpGet]
        [Route(nameof(AvailabilityCheck))]
        public IActionResult AvailabilityCheck()
        {
            return this.Ok();
        }

        [HttpGet]
        [Route(nameof(CurrentVersion))]
        public IActionResult CurrentVersion()
        {
            return this.Ok(Assembly.GetEntryAssembly().GetName().Version.ToString(3));
        }

        [HttpGet(nameof(ShowAllEndpoints))]
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
            });
            return this.Json(output);
        }
    }
}
