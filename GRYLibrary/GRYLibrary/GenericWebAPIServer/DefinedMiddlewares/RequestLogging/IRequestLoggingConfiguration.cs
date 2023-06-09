using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging
{
    public interface IRequestLoggingConfiguration :IMiddlewareConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; }
        public bool LogClientIP { get; set; }
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public bool ShouldBeLogged(Request request);
        public LogLevel GetLogLevel(Request request);
        bool ShouldLogEntireRequestContent(Request request);
        string FormatLogEntryFull(Request request, uint maximalLengthofBodies);
        string FormatTimestamp(DateTime timestamp);
        string Truncate(string value, uint maxLength);
        string FormatLogEntrySummary(Request request);
        string FormatIPAddress(IPAddress clientIP);
    }
    public class Request
    {
        public DateTime Timestamp { get; set; }
        public bool LogClientIP { get; set; }
        public IPAddress ClientIPAddress { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public IHeaderDictionary RequestHeader { get; set; }
        public string RequestBody { get; set; }
        public IDictionary<object, object> InformationFromController { get; set; }
        public ushort ResponseStatusCode { get; set; }
        public IHeaderDictionary ResponseHeader { get; set; }
        public string ResponseBody { get; set; }
        public Request(DateTime timestamp, IPAddress clientIPAddress, string method, string route, IHeaderDictionary requestHeader, string requestBody, IDictionary<object, object> informationFromController, ushort responseStatusCode, IHeaderDictionary responseHeader, string responseBody)
        {
            this.Timestamp = timestamp;
            this.ClientIPAddress = clientIPAddress;
            this.Method = method;
            this.Route = route;
            this.RequestHeader = requestHeader;
            this.RequestBody = requestBody;
            this.InformationFromController = informationFromController;
            this.ResponseStatusCode = responseStatusCode;
            this.ResponseHeader = responseHeader;
            this.ResponseBody = responseBody;
        }
    }
}