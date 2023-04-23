using GRYLibrary.Core.Miscellaneous.FilePath;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public interface IRequestLoggingSettings :IMiddlewareSettings
    {
        public AbstractFilePath WebServerAccessLogFile { get; set; }
        public string FormatRequest(Request request, LogLevel logLevel, bool logEntireRequestContent);
        public bool ShouldBeLogged(Request request);
        public LogLevel GetLogLevel(Request request);
        bool ShouldLogEntireRequestContent(Request request);
    }
    public class Request
    {
        public IPAddress ClientIPAddress { get; set; }
        public string Route { get; set; }
        public string[] RequestHeader { get; set; }
        public string RequestBody { get; set; }
#pragma warning disable CS8632
        public IDictionary<object, object?> InformationFromController { get; set; }
#pragma warning restore CS8632
        public ushort ResponseStatusCode { get; set; }
        public string[] ResponseHeader { get; set; }
        public string ResponseBody { get; set; }
        public Request(IPAddress clientIPAddress, string route, string[] requestHeader, string requestBody, IDictionary<object, object> informationFromController, ushort responseStatusCode, string[] responseHeader, string responseBody)
        {
            this.ClientIPAddress = clientIPAddress;
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