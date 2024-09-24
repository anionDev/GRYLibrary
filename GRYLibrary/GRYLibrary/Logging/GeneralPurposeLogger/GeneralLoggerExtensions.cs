using Microsoft.Extensions.Logging;
using System;
using GUtilies = GRYLibrary.Core.Misc.Utilities;
using System.Diagnostics;
using GRYLibrary.Core.Logging.GRYLogger;
using System.Collections.Generic;

namespace GRYLibrary.Core.Logging.GeneralPurposeLogger
{
    public static class GeneralLoggerExtensions
    {
        public static void Log(this IGeneralLogger logger, string message, LogLevel logLevel)
        {
            LogItem logItem = new LogItem(message, logLevel);
            logger.AddLogEntry(logItem);
        }
        public static void Log(this IGeneralLogger logger, string actionName, LogLevel logLevelForOverhead, bool throwExceptionIfOccurrs, bool logStartOfAction, bool logExceptionOfAtion, bool logEndOfAtion, bool printDuration, Action action)
        {
            if (logStartOfAction)
            {
                logger.Log($"Start action \"{actionName}\".", logLevelForOverhead);
            }
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
                if (logExceptionOfAtion)
                {
                    logger.LogException(exception, $"Error in action \"{actionName}\".");
                }
                if (throwExceptionIfOccurrs)
                {
                    throw;
                }
            }
            finally
            {
                if (logEndOfAtion)
                {
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
            }
        }
        public static void LogLoopExecution<T>(this IGeneralLogger logger, IEnumerable<T> items, Action<T> action)
        {
            throw new NotImplementedException();
        }

        public static void LogException(this IGeneralLogger logger, Exception exception, string message)
        {
            logger.LogException(exception, message, LogLevel.Error);
        }

        public static void LogException(this IGeneralLogger logger, Exception exception, string message, LogLevel logLevel)
        {
            LogItem logItem = new LogItem(message, logLevel, exception);
            logger.AddLogEntry(logItem);
        }

        public static IGRYLog SetupLogger(GRYLogConfiguration configuration, string basePath, string subnamespace)
        {
            GRYLog result = GRYLog.Create(configuration);
            result.UseSubNamespace(subnamespace);
            result.BasePath = basePath;
            return result;
        }
    }
}
