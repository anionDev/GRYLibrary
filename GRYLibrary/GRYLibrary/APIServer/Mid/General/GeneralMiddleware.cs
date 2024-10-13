using GRYLibrary.Core.APIServer.MidT.General;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.General
{
    public class GeneralMiddleware<PersistedApplicationSpecificConfiguration> : GeneralMiddlewareT
        where PersistedApplicationSpecificConfiguration : new()
    {
        private readonly IGRYLog _Log;
        private readonly IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> _PersistedAPIServerConfiguration;
        public GeneralMiddleware(RequestDelegate next, IGRYLog log, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedAPIServerConfiguration) : base(next)
        {
            _Log = log;
            this._PersistedAPIServerConfiguration = persistedAPIServerConfiguration;
        }

        public override Task Invoke(HttpContext context)
        {
            context.Items["ClientIPAddress"] = this.GetIPAddress(context);
            return this._Next(context);
        }

        protected IPAddress? GetIPAddress(HttpContext context)
        {
            IPAddress? result = context.Connection.RemoteIpAddress;
            if (this._PersistedAPIServerConfiguration.ServerConfiguration.TrustForwardedHeader)
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value) && (value != default(string)))
                {
                    _Log.Log($"Retrieved IP by X-Forwarded-For-header which changed the result-ip-address from {result} to '{value}'.",Microsoft.Extensions.Logging.LogLevel.Debug);
                    result = IPAddress.Parse((string)value!);
                }
            }
            if (result != null)
            {
                if (result.IsIPv4MappedToIPv6)
                {
                    result = result.MapToIPv4();
                }
            }
            return result;
        }
    }
}
