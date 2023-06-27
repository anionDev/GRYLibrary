using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.GenericWebAPIServer.Settings.CommonRoutes;
using GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    [ApiController]
    [Route($"{ServerConfiguration.APIRoutePrefix}/APIDocumentation/Information")]
    public class OtherURLs :ControllerBase
    {
        private readonly IApplicationConstants _Configuration;
        public OtherURLs(IApplicationConstants configuration)
        {
            this._Configuration = configuration;
        }

        [HttpGet]
        [Route(nameof(TermsOfService))]
        public IActionResult TermsOfService()
        {
            return this._Configuration.CommonRoutes.Accept(new GetCommonRouteValueVisitor((hostCommonRoutesInformation) => hostCommonRoutesInformation.TermsOfServiceRequestResult));
        }

        [HttpGet]
        [Route(nameof(Contact))]
        public IActionResult Contact()
        {
            return this._Configuration.CommonRoutes.Accept(new GetCommonRouteValueVisitor((hostCommonRoutesInformation) => hostCommonRoutesInformation.ContactRequestResult));
        }

        [HttpGet]
        [Route(nameof(License))]
        public IActionResult License()
        {
            return this._Configuration.CommonRoutes.Accept(new GetCommonRouteValueVisitor((hostCommonRoutesInformation) => hostCommonRoutesInformation.LicenseRequestResult));
        }
        private class GetCommonRouteValueVisitor :ICommonRoutesInformationVisitor<IActionResult>
        {
            private readonly Func<HostCommonRoutes, IActionResult> _CalculateResultFunction;

            public GetCommonRouteValueVisitor(Func<HostCommonRoutes, IActionResult> calculateResultFunction)
            {
                this._CalculateResultFunction = calculateResultFunction;
            }

            public IActionResult Handle(DoNotHostCommonRoutes doNotHostCommonRoutes)
            {
                return new ContentResult() { StatusCode = (int)HttpStatusCode.NotFound };
            }

            public IActionResult Handle(HostCommonRoutes hostCommonRoutes)
            {
                return this._CalculateResultFunction(hostCommonRoutes);
            }
        }
    }
}
