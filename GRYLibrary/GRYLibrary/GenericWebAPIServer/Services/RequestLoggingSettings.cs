using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class RequestLoggingSettings :IRequestLoggingSettings
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