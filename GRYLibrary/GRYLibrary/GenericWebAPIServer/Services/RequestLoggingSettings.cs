using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class RequestLoggingSettings : IRequestLoggingSettings
    {
        public string WebServerAccessLogFile { get; set; }
        public bool Enabled { get; set; } = false;

        public RequestLoggingSettings()
        {
        }

        public virtual string FormatRequest(Request request, LogLevel logLevel, bool logEntireRequestContent)
        {
            throw new NotImplementedException();
        }

        public virtual bool ShouldBeLogged(Request request)
        {
            return true;
        }

        public virtual LogLevel GetLogLevel(Request request)
        {
            return (request.ResponseStatusCode / 100) switch
            {
                2 => LogLevel.Debug,
                3 => LogLevel.Debug,
                4 => LogLevel.Debug,
                5 => LogLevel.Warning,
                _ => LogLevel.Error,
            };
        }

        public bool ShouldLogEntireRequestContent(Request request)
        {
            return request.ResponseStatusCode / 100 == 5;
        }
    }
}
