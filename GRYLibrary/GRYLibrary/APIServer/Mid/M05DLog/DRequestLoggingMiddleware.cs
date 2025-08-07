using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.APIServer.Settings;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using System.Collections.Generic;
using System.Text;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.APIServer.MidT.RLog;
using System.Text.RegularExpressions;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.MidT.Auth;
using System.Security.Claims;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Mid.M05DLog
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
        public DRequestLoggingMiddleware(RequestDelegate next, IDRequestLoggingConfiguration requestLoggingSettings, IApplicationConstants appConstants, IGeneralLogger logger, ITimeService timeService) : base(next, timeService)
        {
            this._RequestLoggingSettings = requestLoggingSettings;
            this._AppConstants = appConstants;
            this._Logger = logger;
            this._RequestLogger = this._AppConstants.ExecutionMode.Accept(new GetLoggerVisitor(this._RequestLoggingSettings.RequestsLogConfiguration, this._AppConstants.GetLogFolder(), "Requests"));
        }
        /// <inheritdoc/>

        protected override void Log(HttpContext context, byte[] requestBodyBytes, byte[] responseBodyBytes)
        {
            try
            {
                DateTimeOffset moment = GUtilities.GetNow();
                (string info, string content, byte[] plainContent) requestBody = BytesToString(requestBodyBytes, this._Encoding);
                (string info, string content, byte[] plainContent) responseBody = BytesToString(responseBodyBytes, this._Encoding);
                string requestRoute = context.Request.Path;
                ushort responseHTTPStatusCode = (ushort)context.Response.StatusCode;
                IPAddress? clientIP = (IPAddress?)context.Items["ClientIPAddress"];
                Request request = new Request(moment, clientIP, context.Request.Method, requestRoute, context.Request.Query, context.Request.Headers, requestBody, null/*TODO*/, responseHTTPStatusCode, context.Response.Headers, responseBody);
                TimeSpan? duration = context.Items.ContainsKey("Duration") ? (TimeSpan)context.Items["Duration"] : default;
                bool isAuthenticated;
                if (context.Items.ContainsKey(AuthenticationMiddleware.IsAuthenticatedInformationName))
                {
                    isAuthenticated = (bool)context.Items[AuthenticationMiddleware.IsAuthenticatedInformationName];
                }
                else
                {
                    isAuthenticated = false;
                }
                ClaimsPrincipal principal = isAuthenticated && context.User != null && context.User.Identity.IsAuthenticated ? context.User : null;
                //TODO add option to add this log-entry to a database
                IDictionary<string, IList<string?>> header = this.GetHeader(context.Request);
                this.LogHTTPRequest(request, false, duration, principal, new HashSet<GRYLogTarget> { new Logging.GRYLogger.ConcreteLogTargets.Console() }, header);
                this.LogHTTPRequest(request, this.ShouldLogEntireRequestContentInLogFile(request), duration, principal, new HashSet<GRYLogTarget> { new Logging.GRYLogger.ConcreteLogTargets.LogFile() }, header);
            }
            catch
            {
                throw;
            }
        }

        private IDictionary<string, IList<string?>> GetHeader(HttpRequest request)
        {
            Dictionary<string, IList<string?>> result = new Dictionary<string, IList<string?>>();
            foreach (string headerToLog in this._RequestLoggingSettings.LoggedHTTPRequeustHeader)
            {
                if (!result.TryGetValue(headerToLog, out IList<string?>? value))
                {
                    value = new List<string?>();
                    result.Add(headerToLog, value);
                }
                if (request.Headers.TryGetValue(headerToLog, out Microsoft.Extensions.Primitives.StringValues values))
                {
                    foreach (string? item in values)
                    {
                        value.Add(item);
                    }
                }
            }
            return result;
        }

        public static (string info, string content, byte[] plainContent) BytesToString(byte[] content, Encoding encoding)
        {
            if (content.Length == 0)
            {
                return ("Empty", null, content);
            }
            try
            {
                return ("UTF8-encoded-content", encoding.GetString(content), content);
            }
            catch
            {
                return ("Hex-encoded-content", GUtilities.ByteArrayToHexString(content), content);
            }
        }

        private void LogHTTPRequest(Request request, bool logFullRequest, TimeSpan? duration, ClaimsPrincipal user, ISet<GRYLogTarget> logTargets, IDictionary<string, IList<string?>> header)
        {
            try
            {
                if (this.ShouldBeLogged(request))
                {
                    LogLevel logLevel = this.GetLogLevel(request);
                    string formatted;
                    if (logFullRequest)
                    {
                        formatted = this.FormatLogEntryFull(request, duration, user, this._RequestLoggingSettings.MaximalLengthofRequestBodies, this._RequestLoggingSettings.MaximalLengthOfResponseBodies, header);
                    }
                    else
                    {
                        formatted = this.FormatLogEntrySummary(request, duration, user);
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
                3 => LogLevel.Debug,
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
            foreach (string notLoggedRoute in this._RequestLoggingSettings.NotLoggedRoutes)
            {
                if (Regex.IsMatch(route, notLoggedRoute))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual string FormatLogEntryFull(Request request, TimeSpan? duration, ClaimsPrincipal user, uint maximalLengthofRequestBodies, uint maximalLengthofResponseBodies, IDictionary<string, IList<string?>> header)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            string result = $"Request received:{Environment.NewLine}"
                        + $"  Timestamp: {this.FormatTimestamp(request.Timestamp)}{Environment.NewLine}"
                        + $"  Client-ip: {clientIPAsString}{Environment.NewLine}"
                        + $"  Request-details:{Environment.NewLine}"
                        + $"    Method: {request.Method}{Environment.NewLine}"
                        + $"    Route: {request.Route}{request.GetFormattedQuery()}{Environment.NewLine}";
            if (0 < header.Count)
            {
                result = result + $"  Header:{Environment.NewLine}";
                foreach (KeyValuePair<string, IList<string?>> kvp in header)
                {
                    string value = "{" + string.Join(", ", kvp.Value.Select(item => "\"" + item + "\"")) + "}";
                    result = result + $"    - {kvp.Key}: {value}{Environment.NewLine}";
                }
            }
            result = result + $"    Body: {this.FormatBody(request.RequestBody, maximalLengthofRequestBodies)}{Environment.NewLine}"
                        + $"  Response-details:{Environment.NewLine}"
                        + $"    Statuscode: {request.ResponseStatusCode}{Environment.NewLine}"
                        + $"    Body: {this.FormatBody(request.ResponseBody, maximalLengthofResponseBodies)}{Environment.NewLine}";
            if (user == null)
            {
                result = result + $"  Authentication: (anonymous){Environment.NewLine}";
            }
            else
            {
                result = result + $"  Authentication: user \"{user.Identity.Name}\"{Environment.NewLine}";
            }
            if (duration.HasValue)
            {
                result = result + $"  Duration: {GUtilities.DurationToUserFriendlyString(duration.Value, 5)}{Environment.NewLine}";
            }
            return result;
        }

        private string FormatBody((string info, string content, byte[] plainContent) body, uint maximalLengthofRequestBodies)
        {
            string result;
            if (body.content == null)
            {
                result = body.info;
            }
            else
            {
                result = $"{body.info} ({this.Truncate(body.content, maximalLengthofRequestBodies, (ulong)body.plainContent.LongLength)})";
            }
            return result;
        }

        public virtual bool ShouldBeLogged(Request request)
        {
            if (request.ResponseStatusCode / 100 == 5)
            {
                return true;
            }
            if (this.IsIgnored(request.Route))
            {
                return false;
            }
            return true;
        }
        public virtual string FormatLogEntrySummary(Request request, TimeSpan? duration, ClaimsPrincipal user)
        {
            string clientIPAsString = this.FormatIPAddress(request.ClientIPAddress);
            string additionalInformationFinal;
            string additionalInformation = this.GetAdditionalInformation(request, clientIPAsString, duration, user);
            if (additionalInformation == null)
            {
                additionalInformation = string.Empty;
            }
            else
            {
                additionalInformation = $" ({additionalInformation})";
            }
            additionalInformationFinal = additionalInformation;
            return $"Request received: {this.FormatTimestamp(request.Timestamp)} {clientIPAsString} requested \"{request.Method} {request.Route}{request.GetFormattedQuery()}\" and got response-code {request.ResponseStatusCode}.{additionalInformationFinal}";
        }

        public virtual string GetAdditionalInformation(Request request, string clientIPAsString, TimeSpan? duration, ClaimsPrincipal user)
        {
            string result = null;
            if (user == null)
            {
                result = this.AddAdditionalInformtion(result, $"Authentication: (anonymous)");
            }
            else
            {
                result = this.AddAdditionalInformtion(result, $"Authentication: user \"{user.Identity.Name}\"");
            }
            if (duration.HasValue)
            {
                result = this.AddAdditionalInformtion(result, $"Duration: {GUtilities.DurationToUserFriendlyString(duration.Value, 3)}");
            }
            //add more additional information if desired
            return result;
        }

        private string AddAdditionalInformtion(string result, string message)
        {

            if (result == null)
            {
                return message;
            }
            else
            {
                return $"{result}; {message}";
            }
        }

        public virtual string FormatTimestamp(DateTimeOffset timestamp)
        {
            return GUtilities.FormatTimestamp(timestamp, this._RequestLoggingSettings.AddMillisecondsInLogTimestamps);
        }

        public virtual string Truncate(string value, uint maxLength, ulong originalLength)
        {
            if (value.Length <= maxLength)
            {
                return $"\"{value}\"";
            }
            else
            {
                return $"\"{value[..(int)maxLength]}...\" (Content truncated; Original length: {originalLength} bytes)";
            }
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