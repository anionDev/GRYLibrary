using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class RequestLoggingMiddleware :AbstractMiddleware
    {
        private readonly IGeneralLogger _Logger;
        private readonly IRequestLoggingSettings _RequestLoggingSettings;
        /// <inheritdoc/>
        public RequestLoggingMiddleware(RequestDelegate next, IRequestLoggingSettings requestLoggingSettings, IWebAPIConfigurationConstants webAPIConfigurationConstants) : base(next)
        {
            this._RequestLoggingSettings = requestLoggingSettings;
            this._Logger = GeneralLogger.CreateUsingGRYLog(GetLogConfiguration(requestLoggingSettings.WebServerAccessLogFile, webAPIConfigurationConstants.TargetEnvironmentType));
        }
        private static GRYLogConfiguration GetLogConfiguration(AbstractFilePath webServerAccessLogFile, GRYEnvironment environment)
        {
            GRYLogConfiguration logConfig = new GRYLogConfiguration(true);
            LogFile filelog = new LogFile
            {
                Format = GRYLogLogFormat.DateOnly,
                File = webServerAccessLogFile,
                Enabled = true
            };
            logConfig.LogTargets = new List<GRYLogTarget> { filelog };
            if(environment is not Productive)
            {
                logConfig.LogTargets.Add(new Console());
            }
            return logConfig;
        }  /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            bool implemented = false;
            if(implemented)
            {
                Request request = default;//TODO create real object
                if(this._RequestLoggingSettings.ShouldBeLogged(request))
                {
                    LogLevel logLevel = this._RequestLoggingSettings.GetLogLevel(request);
                    string formatted = this._RequestLoggingSettings.FormatRequest(request, logLevel, this._RequestLoggingSettings.ShouldLogEntireRequestContent(request));
                    LogItem logItem = new LogItem(formatted, logLevel);
                    this._Logger.AddLogEntry(logItem);
                }
            }
            return this._Next(context);
        }

        public virtual LogLevel GetLogLevelForRequestLogEntry(string route, ushort responseStatusCode)
        {
            if(responseStatusCode % 100 == 5)
            {
                return LogLevel.Error;
            }
            else
            {
                return LogLevel.Information;
            }
        }
        public virtual ushort GetEventLogIdForRequestLogEntry(string route, ushort responseStatusCode)
        {
            return 9700;
        }
    }
}