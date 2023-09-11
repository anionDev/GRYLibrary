using GRYLibrary.Core.Log;
using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.GeneralPurposeLogger
{
    public static class GeneralLoggerExtensions
    {
        public static void Log(this IGeneralLogger logger, string message, LogLevel logLevel)
        {
            LogItem logItem = new LogItem(message, logLevel);
            logger.AddLogEntry(logItem);
        }
        public static void Log(this IGeneralLogger logger, string actionName, bool throwException, Action action)
        {
            logger.Log($"Started \"{actionName}\"", LogLevel.Debug);
            try
            {
                action();
            }
            catch (Exception exception)
            {
                logger.LogException(exception, $"Error in \"{actionName}\"");
                if (throwException)
                {
                    throw;
                }
            }
            logger.Log($"Finished \"{actionName}\"", LogLevel.Debug);
        }
        public static void LogException(this IGeneralLogger logger, Exception exception, string message)
        {
            LogItem logItem = new LogItem(message, exception);
            logger.AddLogEntry(logItem);
        }
    }
}
