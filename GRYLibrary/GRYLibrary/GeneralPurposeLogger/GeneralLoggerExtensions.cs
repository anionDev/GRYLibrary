using GRYLibrary.Core.Log;
using Microsoft.Extensions.Logging;
using System;
using GUtilies = GRYLibrary.Core.Miscellaneous.Utilities;
using System.Diagnostics;

namespace GRYLibrary.Core.GeneralPurposeLogger
{
    public static class GeneralLoggerExtensions
    {
        public static void Log(this IGeneralLogger logger, string message, LogLevel logLevel)
        {
            LogItem logItem = new LogItem(message, logLevel);
            logger.AddLogEntry(logItem);
        }
        public static void Log(this IGeneralLogger logger, string actionName, LogLevel logLevelForOverhead, bool throwException, bool printDuration, Action action)
        {
            logger.Log($"Start action \"{actionName}\".", logLevelForOverhead);
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                stopwatch.Start();
                action();
                stopwatch.Stop();
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                logger.LogException(exception, $"Error in action \"{actionName}\".");
                if (throwException)
                {
                    throw;
                }
            }
            string duration;
            if (printDuration)
            {
                duration = $" Duration: {GUtilies.DurationToUserFriendlyString(stopwatch.Elapsed)}";
            }
            else
            {
                duration = GUtilies.EmptyString;
            }
            logger.Log($"Finished action \"{actionName}\".{duration}", logLevelForOverhead);
        }
        public static void LogException(this IGeneralLogger logger, Exception exception, string message)
        {
            LogItem logItem = new LogItem(message, exception);
            logger.AddLogEntry(logItem);
        }

        public static GRYLog SetupLogger(GRYLogConfiguration configuration, string basePath, string subnamespace)
         {
            GRYLog result = GRYLog.Create(configuration);
            result.UseSubNamespace(subnamespace);
            result.BasePath = basePath;
            return result;
        }
    }
}
