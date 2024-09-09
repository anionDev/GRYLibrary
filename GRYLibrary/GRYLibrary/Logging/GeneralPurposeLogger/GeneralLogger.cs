using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.IO;

namespace GRYLibrary.Core.Logging.GeneralPurposeLogger
{
    public class GeneralLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static IGeneralLogger CreateUsingGRYLog(GRYLogConfiguration configuration, out GRYLog logger) => CreateUsingGRYLog(configuration, out logger, Directory.GetCurrentDirectory());
        public static IGeneralLogger CreateUsingGRYLog(GRYLogConfiguration configuration, out GRYLog logger, string basePath = null)
        {
            GRYLog logObject = GRYLog.Create(configuration);
            logObject.BasePath = basePath;
            logger = logObject;
            return new GeneralLogger()
            {
                AddLogEntry = logObject.Log
            };
        }

        public static GeneralLogger NoLog() => new GeneralLogger() { AddLogEntry = (logItem) => { } };
        public static IGeneralLogger CreateUsingConsole() => CreateUsingGRYLog(new GRYLogConfiguration(true), out _);
    }
}