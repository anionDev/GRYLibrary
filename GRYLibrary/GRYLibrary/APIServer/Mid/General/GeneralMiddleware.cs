using GRYLibrary.Core.APIServer.MidT.General;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.General
{
    internal class GeneralMiddleware<PersistedApplicationSpecificConfiguration> : GeneralMiddlewareT
        where PersistedApplicationSpecificConfiguration : new()
    {
        private readonly IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> _PersistedAPIServerConfiguration;
        public GeneralMiddleware(RequestDelegate next, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedAPIServerConfiguration) : base(next)
        {
            _PersistedAPIServerConfiguration = persistedAPIServerConfiguration;
        }

        public override Task Invoke(HttpContext context)
        {
            context.Items["RemoteIPAddress"] = this.GetIPAddress(context);
            return this._Next(context);
        }

        private IPAddress GetIPAddress(HttpContext context)
        {
            IPAddress result = context.Connection.RemoteIpAddress;
            if (_PersistedAPIServerConfiguration.ServerConfiguration.TrustForwardedHeader)
            {
                throw new NotImplementedException();//TODO process forwardheader 
            }
            return result;
        }
    }
}
