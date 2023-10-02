using GRYLibrary.Core.Log;
using System;
using System.IO;

namespace GRYLibrary.Core.GeneralPurposeLogger
{
    public class GeneralLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GeneralLogger CreateUsingGRYLog(GRYLogConfiguration configuration, out GRYLog logger)
        {
            return CreateUsingGRYLog(configuration, out logger, Directory.GetCurrentDirectory());
        }
        public static GeneralLogger CreateUsingGRYLog(GRYLogConfiguration configuration, out GRYLog logger, string basePath = null)
        {
            GRYLog logObject = GRYLog.Create(configuration);
            logObject.BasePath = basePath;
            logger = logObject;
            return new GeneralLogger()
            {
                AddLogEntry = logObject.Log
            };
        }

        public static GeneralLogger NoLog()
        {
            return new GeneralLogger() { AddLogEntry = (logItem) => { } };
        }
        public static GeneralLogger CreateUsingConsole()
        {
            return CreateUsingGRYLog(new GRYLogConfiguration(true), out _);
        }
    }
}