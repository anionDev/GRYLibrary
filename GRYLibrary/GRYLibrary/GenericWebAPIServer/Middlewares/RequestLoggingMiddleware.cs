using GRYLibrary.Core.GeneralPurposeLogger;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Text;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.GenericWebAPIServer.Utilities;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class RequestLoggingMiddleware<PersistedApplicationSpecificConfiguration, AppConstants> :AbstractMiddleware
        where PersistedApplicationSpecificConfiguration : new()
    {
        private readonly IGeneralLogger _RequestLogger;
        private readonly IGeneralLogger _ServerLogger;
        private readonly IRequestLoggingSettings _RequestLoggingSettings;
        private readonly IApplicationConstants<AppConstants> _AppConstants;
        /// <inheritdoc/>
        public RequestLoggingMiddleware(RequestDelegate next, IRequestLoggingSettings requestLoggingSettings, IApplicationConstants<AppConstants> appConstants, IGeneralLogger serverLogger) : base(next)
        {
            this._RequestLoggingSettings = requestLoggingSettings;
            this._AppConstants = appConstants;
            this._ServerLogger = serverLogger;
            this._RequestLogger = this._AppConstants.ExecutionMode.Accept(new GetLoggerVisitor(this._RequestLoggingSettings.RequestsLogConfiguration, this._AppConstants.GetLogFolder(), "Requests"));
        }
        /// <inheritdoc/>
        public async override Task Invoke(HttpContext context)
        {
            DateTime moment = DateTime.Now;
            UTF8Encoding encoding = new UTF8Encoding(false);
            HttpRequestRewindExtensions.EnableBuffering(context.Request);
            string requestBody = await new StreamReader(context.Request.Body, encoding, false).ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            Stream originalBodyStream = context.Response.Body;
            using MemoryStream responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;
            await this._Next(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(context.Response.Body, encoding, false).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
            this.LogHTTPRequest(context, requestBody, responseBody, moment);
        }

        private void LogHTTPRequest(HttpContext context, string requestBody, string responseBody, DateTime timestamp, uint maximalLengthofBodies = 4096)
        {
            try
            {
                string requestRoute = context.Request.Path;
                ushort responseHTTPStatuscode = (ushort)context.Response.StatusCode;
                IPAddress clientIP = context.Connection.RemoteIpAddress;
                Request request = new Request(timestamp, clientIP, context.Request.Method, requestRoute, context.Request.Headers, requestBody, null/*TODO*/, responseHTTPStatuscode, context.Response.Headers, responseBody);
                if(_RequestLoggingSettings.ShouldBeLogged(request))
                {
                    LogLevel logLevel = this._RequestLoggingSettings.GetLogLevel(request);
                    string formatted;
                    if(this._RequestLoggingSettings.ShouldLogEntireRequestContent(request))
                    {
                        formatted = this.FormatLogEntryFull(request, maximalLengthofBodies);
                    }
                    else
                    {
                        formatted = this.FormatLogEntrySummary(request);
                    }
                    this._RequestLogger.Log(formatted, logLevel);
                }
            }
            catch(Exception exception)
            {
                _RequestLogger.LogException(exception, "Error while logging request.");
            }
        }

        internal string FormatLogEntryFull(Request request, uint maximalLengthofBodies)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            return $"Request:{Environment.NewLine}"
                        + $"  Timestamp: {request.Timestamp:o}{Environment.NewLine}"
                        + $"  Client-ip: {clientIPAsString}{Environment.NewLine}"
                        + $"  Request-details:{Environment.NewLine}"
                        + $"    Method: {request.Route}{Environment.NewLine}"
                        + $"    Route: {request.Method}{Environment.NewLine}"
                        + $"    Body: {this.Truncate(request.RequestBody, maximalLengthofBodies)}{Environment.NewLine}"
                        + $"  Response-details:{Environment.NewLine}"
                        + $"    Statuscode: {request.ResponseStatusCode}{Environment.NewLine}"
                        + $"    Body: {this.Truncate(request.ResponseBody, maximalLengthofBodies)}{Environment.NewLine}";
        }

        private string FormatIPAddress(IPAddress clientIP)
        {
            return this._RequestLoggingSettings.LogClientIP ? "(IP-address not saved)" : (clientIP == null ? clientIP.ToString() : "(IP-address not available)");
        }

        internal string FormatLogEntrySummary(Request request)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            return $"Request: {request.Timestamp:o} {clientIPAsString} requested \"{request.Method} {request.Route}\" and got response-code {request.ResponseStatusCode}.";
        }
        internal string Truncate(string value, uint maxLength)
        {
            return string.IsNullOrEmpty(value) || value.Length <= maxLength ? value : value[..(int)maxLength];
        }
    }
}