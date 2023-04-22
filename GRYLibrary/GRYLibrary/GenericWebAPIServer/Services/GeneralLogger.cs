using GRYLibrary.Core.Graph;
using GRYLibrary.Core.Log;
using System;
using System.IO;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class GeneralLogger :IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GeneralLogger CreateUsingGRYLog(GRYLogConfiguration configuration)
        {
          return  CreateUsingGRYLog(configuration, Directory.GetCurrentDirectory());
        }
        public static GeneralLogger CreateUsingGRYLog(GRYLogConfiguration configuration, string basePath = null)
        {
            GRYLog logObject = GRYLog.Create(configuration);
            logObject.BasePath = basePath;
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
            return CreateUsingGRYLog(new GRYLogConfiguration(true));
        }
    }
}