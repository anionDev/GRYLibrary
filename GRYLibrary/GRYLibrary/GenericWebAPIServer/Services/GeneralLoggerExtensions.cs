using GRYLibrary.Core.Log;
using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public static class GeneralLoggerExtensions
    {
        public static void Log(this IGeneralLogger logger, string message, LogLevel logLevel)
        {
            LogItem logItem = new LogItem(message, logLevel);
            logger.AddLogEntry(logItem);
        }
        public static void LogException(this IGeneralLogger logger, Exception exception, string message)
        {
            LogItem logItem = new LogItem(message, exception);
            logger.AddLogEntry(logItem);
        }
    }
}
