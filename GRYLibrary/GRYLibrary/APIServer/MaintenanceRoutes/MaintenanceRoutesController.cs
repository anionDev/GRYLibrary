using Microsoft.AspNetCore.Mvc;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using GRYLibrary.Core.Misc;
using System.IO;

namespace GRYLibrary.Core.APIServer.MaintenanceRoutes
{
    [ApiController]
    [Route($"{ServerConfiguration.APIRoutePrefix}/Other/Maintenance")]
    public class MaintenanceRoutesController : Controller
    {
        private readonly IApplicationConstants _Configuration;
        private readonly IMaintenanceRoutesInformation _MaintenanceRoutesInformation;
        private readonly IEnumerable<EndpointDataSource> _EndpointSources;
        private readonly IHealthCheck _HealthCheck;
        public MaintenanceRoutesController(IApplicationConstants configuration, IMaintenanceRoutesInformation maintenanceRoutesInformation, IEnumerable<EndpointDataSource> endpointSources, IHealthCheck healthCheck)
        {
            this._Configuration = configuration;
            this._MaintenanceRoutesInformation = maintenanceRoutesInformation;
            this._EndpointSources = endpointSources;
            this._HealthCheck = healthCheck;
        }

        [HttpGet]
        [Route(nameof(AvailabilityCheck))]
        public virtual IActionResult AvailabilityCheck()
        {
            return this.Ok();
        }

        [HttpGet]
        [Route(nameof(CurrentVersion))]
        public virtual IActionResult CurrentVersion()
        {
            return this.Ok(Assembly.GetEntryAssembly().GetName().Version.ToString(3));
        }

        [HttpGet(nameof(ShowAllEndpoints))]
        public virtual IActionResult ShowAllEndpoints()
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

        [HttpGet]
        [Route(nameof(HealthCheck))]
        ///<returns>
        ///Returns a JSON with a "status"-property.
        ///Status-meaning:
        ///0 = Unhealthy
        ///1 = Degraded
        ///2 = Healthy
        ///</returns>
        ///<example>
        ///{"data":{},"description":"Service is healthy.","exception":null,"status":2}
        /// </example>
        public virtual IActionResult HealthCheck()
        {
            return this.Ok(this._HealthCheck.CheckHealthAsync(new HealthCheckContext()).WaitAndGetResult());
        }

        [HttpGet]
        [Route(nameof(Metrics))]
        public virtual IActionResult Metrics()
        {
            using MemoryStream ms = new MemoryStream();
            Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(ms);
            ms.Position = 0;
            using StreamReader sr = new StreamReader(ms);
            var allmetrics = sr.ReadToEndAsync().WaitAndGetResult();
            return this.Ok(allmetrics);
        }
    }
}
