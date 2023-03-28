using GRYLibrary.Core.Log;
using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public interface IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static void Log(string message, LogLevel logLevel, IGeneralLogger logger)
        {
            LogItem logItem = new LogItem(message, logLevel);
            logger.AddLogEntry(logItem);
        }
        public static void LogException(Exception exception, string message, IGeneralLogger logger)
        {
            LogItem logItem = new LogItem(message, exception);
            logger.AddLogEntry(logItem);
        }
    }
}