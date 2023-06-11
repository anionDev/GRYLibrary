using GRYLibrary.Core.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using System.Threading.Tasks;
using GRYLibrary.Core.GenericWebAPIServer.Utilities;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging;
using GRYLibrary.Core.GenericWebAPIServer.Settings;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogger
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class RequestLoggingMiddleware :AbstractMiddleware
    {
        private readonly IGeneralLogger _RequestLogger;
        private readonly IGeneralLogger _Logger;
        private readonly IRequestLoggingConfiguration _RequestLoggingSettings;
        private readonly IApplicationConstants _AppConstants;
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
            DateTime moment = DateTime.Now;
            string requestBody = Tools.GetRequestBodyAsString(context);
            Task result = this._Next(context);
            string responseBody = Tools.GetResponseBodyAsString(context);
            this.LogHTTPRequest(context, requestBody, responseBody, moment);
            return result;
        }

        private void LogHTTPRequest(HttpContext context, string requestBody, string responseBody, DateTime timestamp, uint maximalLengthofBodies = 4096)
        {
            try
            {
                string requestRoute = context.Request.Path;
                ushort responseHTTPStatuscode = (ushort)context.Response.StatusCode;
                IPAddress clientIP = context.Connection.RemoteIpAddress;
                Request request = new Request(timestamp, clientIP, context.Request.Method, requestRoute, context.Request.Headers, requestBody, null/*TODO*/, responseHTTPStatuscode, context.Response.Headers, responseBody);
                if(this._RequestLoggingSettings.ShouldBeLogged(request))
                {
                    LogLevel logLevel = this._RequestLoggingSettings.GetLogLevel(request);
                    string formatted;
                    if(this._RequestLoggingSettings.ShouldLogEntireRequestContent(request))
                    {
                        formatted = this._RequestLoggingSettings.FormatLogEntryFull(request, maximalLengthofBodies);
                    }
                    else
                    {
                        formatted = this._RequestLoggingSettings.FormatLogEntrySummary(request);
                    }
                    this._RequestLogger.Log(formatted, logLevel);
                }
            }
            catch(Exception exception)
            {
                this._RequestLogger.LogException(exception, "Error while logging request.");
            }
        }
    }
}