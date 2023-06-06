using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using GRYLibrary.Core.Log;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class RequestLoggingSettings :IRequestLoggingSettings
    {
        public bool AddMillisecondsInLogTimestamps { get; set; } = false;
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public bool Enabled { get; set; } = true;
        public bool LogClientIP { get; set; } = true;       

        public virtual bool ShouldBeLogged(Request request)
        {
            return true;
        }

        public virtual LogLevel GetLogLevel(Request request)
        {
            return (request.ResponseStatusCode / 100) switch
            {
                2 => LogLevel.Information,
                3 => LogLevel.Information,
                4 => LogLevel.Information,
                5 => LogLevel.Error,
                _ => LogLevel.Error,
            };
        }

        public bool ShouldLogEntireRequestContent(Request request)
        {
            return request.ResponseStatusCode / 100 == 5;
        }

        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}