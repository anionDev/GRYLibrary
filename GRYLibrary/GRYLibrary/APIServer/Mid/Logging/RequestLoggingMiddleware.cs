using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using System.Threading.Tasks;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.APIServer.Mid.Logging;
using GRYLibrary.Core.APIServer.Settings;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using System.Collections.Generic;
using System.Text;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;

namespace GRYLibrary.Core.APIServer.Mid.RequestLogger
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class RequestLoggingMiddleware : AbstractMiddleware
    {
        private readonly IGeneralLogger _RequestLogger;
        private readonly IGeneralLogger _Logger;
        private readonly IRequestLoggingConfiguration _RequestLoggingSettings;
        private readonly IApplicationConstants _AppConstants;
        private readonly Encoding _Encoding = new UTF8Encoding(false);
        /// <inheritdoc/>
        public RequestLoggingMiddleware(RequestDelegate next, IRequestLoggingConfiguration requestLoggingSettings, IApplicationConstants appConstants, IGeneralLogger logger) : base(next)
        {
            this._RequestLoggingSettings = requestLoggingSettings;
            this._AppConstants = appConstants;
            this._Logger = logger;
            this._RequestLogger = this._AppConstants.ExecutionMode.Accept(new GetLoggerVisitor(this._RequestLoggingSettings.RequestsLogConfiguration, this._AppConstants.GetLogFolder(), "Requests"));
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            DateTime moment = GUtilities.GetNow();
            (byte[] requestBodyB, byte[] responseBodyB) = Tools.ExecuteAndGetBody(this._Next, context);
            string requestBody = this.BytesToString(requestBodyB);
            string responseBody = this.BytesToString(responseBodyB);
            string requestRoute = context.Request.Path;
            ushort responseHTTPStatusCode = (ushort)context.Response.StatusCode;
            IPAddress clientIP = context.Connection.RemoteIpAddress;
            Request request = new Request(moment, clientIP, context.Request.Method, requestRoute, context.Request.Query, context.Request.Headers, requestBody, null/*TODO*/, responseHTTPStatusCode, context.Response.Headers, responseBody);
            this.LogHTTPRequest(request, false, new HashSet<GRYLogTarget> { new Core.Logging.GRYLogger.ConcreteLogTargets.Console() });
            this.LogHTTPRequest(request, this.ShouldLogEntireRequestContentInLogFile(request), new HashSet<GRYLogTarget> { new Core.Logging.GRYLogger.ConcreteLogTargets.LogFile() });
            return Task.CompletedTask;
        }

        private string BytesToString(byte[] content)
        {
            try
            {
                return $"UTF8-encoded-content: \"{this._Encoding.GetString(content)}\"";
            }
            catch
            {
                return $"Hex-encoded-content: {GUtilities.ByteArrayToHexString(content)}";
            }
        }

        private void LogHTTPRequest(Request request, bool logFullRequest, ISet<GRYLogTarget> logTargets)
        {
            try
            {
                if (this.ShouldBeLogged(request))
                {
                    LogLevel logLevel = this.GetLogLevel(request);
                    string formatted;
                    if (logFullRequest)
                    {
                        formatted = this.FormatLogEntryFull(request, this._RequestLoggingSettings.MaximalLengthofRequestBodies, this._RequestLoggingSettings.MaximalLengthofResponseBodies);
                    }
                    else
                    {
                        formatted = this.FormatLogEntrySummary(request);
                    }
                    LogItem logItem = new LogItem(formatted, logLevel)
                    {
                        LogTargets = logTargets
                    };
                    this._RequestLogger.AddLogEntry(logItem);
                }
            }
            catch (System.Exception exception)
            {
                this._Logger.LogException(exception, "Error while logging request.");
            }
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

        public virtual bool ShouldLogEntireRequestContentInLogFile(Request request)
        {
            return request.ResponseStatusCode / 100 == 5;
        }

        public virtual string FormatLogEntryFull(Request request, uint maximalLengthofRequestBodies, uint maximalLengthofResponseBodies)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            return $"Request received:{Environment.NewLine}"
                        + $"  Timestamp: {this.FormatTimestamp(request.Timestamp)}{Environment.NewLine}"
                        + $"  Client-ip: {clientIPAsString}{Environment.NewLine}"
                        + $"  Request-details:{Environment.NewLine}"
                        + $"    Method: {request.Route}{Environment.NewLine}"
                        + $"    Route: {request.Method}{request.GetFormattedQuery()}{Environment.NewLine}"
                        + $"    Body: {this.Truncate(request.RequestBody, maximalLengthofRequestBodies)}{Environment.NewLine}"
                        + $"  Response-details:{Environment.NewLine}"
                        + $"    Statuscode: {request.ResponseStatusCode}{Environment.NewLine}"
                        + $"    Body: {this.Truncate(request.ResponseBody, maximalLengthofResponseBodies)}{Environment.NewLine}";
        }

        public virtual bool ShouldBeLogged(Request request)
        {
            return true;
        }
        public virtual string FormatLogEntrySummary(Request request)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            string additionalInformation = this.GetAdditionalInformation(request, clientIPAsString);
            if (additionalInformation == null)
            {
                additionalInformation = string.Empty;
            }
            else
            {
                additionalInformation = $"Additional information: {additionalInformation}";
            }
            return $"Request received: {this.FormatTimestamp(request.Timestamp)} {clientIPAsString} requested \"{request.Method} {request.Route}{request.GetFormattedQuery()}\" and got response-code {request.ResponseStatusCode}.{additionalInformation}";
        }

        public virtual string GetAdditionalInformation(Request request, string clientIPAsString)
        {
            return null;
        }

        public virtual string FormatTimestamp(DateTime timestamp)
        {
            return GUtilities.FormatTimestamp(timestamp, this._RequestLoggingSettings.AddMillisecondsInLogTimestamps);
        }
        public virtual string Truncate(string value, uint maxLength)
        {
            int contentLength = value.Length;
            if (contentLength <= maxLength)
            {
                return $"<{value}>";
            }
            else
            {
                return $"<{value[..(int)maxLength]}...> (truncated, original length: {contentLength} characters)";
            };
        }
        public virtual string FormatIPAddress(IPAddress clientIP)
        {
            if (this._RequestLoggingSettings.LogClientIP)
            {
                if (clientIP == null)
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
    }
}