using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging
{
    public class RequestLoggingConfiguration :IRequestLoggingConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; } = false;
        public GRYLogConfiguration RequestsLogConfiguration { get; set; } =  GRYLogConfiguration.GetCommonConfiguration(RelativeFilePath.FromString("./Requests.log"));
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

        public virtual bool ShouldLogEntireRequestContent(Request request)
        {
            return request.ResponseStatusCode / 100 == 5;
        }

        public virtual string FormatLogEntryFull(Request request, uint maximalLengthofBodies)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            return $"Request received:{Environment.NewLine}"
                        + $"  Timestamp: {this.FormatTimestamp(request.Timestamp)}{Environment.NewLine}"
                        + $"  Client-ip: {clientIPAsString}{Environment.NewLine}"
                        + $"  Request-details:{Environment.NewLine}"
                        + $"    Method: {request.Route}{Environment.NewLine}"
                        + $"    Route: {request.Method}{Environment.NewLine}"
                        + $"    Body: {this.Truncate(request.RequestBody, maximalLengthofBodies)}{Environment.NewLine}"
                        + $"  Response-details:{Environment.NewLine}"
                        + $"    Statuscode: {request.ResponseStatusCode}{Environment.NewLine}"
                        + $"    Body: {this.Truncate(request.ResponseBody, maximalLengthofBodies)}{Environment.NewLine}";
        }

        public virtual string FormatLogEntrySummary(Request request)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            return $"Request received: {this.FormatTimestamp(request.Timestamp)} {clientIPAsString} requested \"{request.Method} {request.Route}\" and got response-code {request.ResponseStatusCode}.";
        }
        public virtual string FormatTimestamp(DateTime timestamp)
        {
            return Miscellaneous.Utilities.FormatTimestamp(timestamp, this.AddMillisecondsInLogTimestamps);
        }
        public virtual string Truncate(string value, uint maxLength)
        {
            return string.IsNullOrEmpty(value) || value.Length <= maxLength ? value : value[..(int)maxLength];
        }
        public virtual string FormatIPAddress(IPAddress clientIP)
        {
            if(this.LogClientIP)
            {
                if(clientIP == null)
                {
                    return "(IP-address not available)";
                }
                else
                {
                    return clientIP.ToString();
                }
            }
            else
            {
                return "(IP-address not saved)";
            }
        }

        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}