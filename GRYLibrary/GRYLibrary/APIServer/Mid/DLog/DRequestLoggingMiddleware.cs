using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.APIServer.Settings;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using System.Collections.Generic;
using System.Text;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.APIServer.MidT.RLog;
using System.Text.RegularExpressions;

namespace GRYLibrary.Core.APIServer.Mid.DLog
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class DRequestLoggingMiddleware : RequestLoggingMiddleware
    {
        private readonly IGeneralLogger _RequestLogger;
        private readonly IGeneralLogger _Logger;
        private readonly IDRequestLoggingConfiguration _RequestLoggingSettings;
        private readonly IApplicationConstants _AppConstants;
        private readonly Encoding _Encoding = new UTF8Encoding(false);
        /// <inheritdoc/>
        public DRequestLoggingMiddleware(RequestDelegate next, IDRequestLoggingConfiguration requestLoggingSettings, IApplicationConstants appConstants, IGeneralLogger logger) : base(next)
        {
            this._RequestLoggingSettings = requestLoggingSettings;
            this._AppConstants = appConstants;
            this._Logger = logger;
            this._RequestLogger = this._AppConstants.ExecutionMode.Accept(new GetLoggerVisitor(this._RequestLoggingSettings.RequestsLogConfiguration, this._AppConstants.GetLogFolder(), "Requests"));
        }
        /// <inheritdoc/>

        protected override void Log(HttpContext context)
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
            catch (Exception exception)
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
            if (request.ResponseStatusCode / 100 == 5)
            {
                return true;
            }
            return true;
        }

        private bool IsIgnored(string route)
        {
            foreach (var notLoggedRoute in this._RequestLoggingSettings.NotLoggedRoutes)
            {
                if (Regex.IsMatch(route, notLoggedRoute))
                {
                    return true;
                }
            }
            return false;
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
            if (request.ResponseStatusCode / 100 == 5)
            {
                return true;
            }
            if (IsIgnored(request.Route))
            {
                return false;
            }
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