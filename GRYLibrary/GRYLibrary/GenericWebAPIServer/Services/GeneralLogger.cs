using GRYLibrary.Core.Log;
using System;
using System.IO;
using System.Reflection;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class GeneralLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GeneralLogger Create(string appName, string logFolder)
        {
            Miscellaneous.Utilities.EnsureDirectoryExists(logFolder);
            var logObject = GRYLog.Create();
            var initialConfiguration = new GRYLogConfiguration();
            string logFile = Path.Combine(logFolder, $"{appName}.log");
            initialConfiguration.ResetToDefaultValues(logFile);
            logObject.Configuration = Miscellaneous.Utilities.CreateOrLoadLoadXMLConfigurationFile("LogSettings.xml", initialConfiguration);
            return new GeneralLogger()
            {
                AddLogEntry = (logEntry) =>
                {
                    logObject.Log(logEntry);
                }
            };
        }
        public static GeneralLogger NoLog()
        {
           return new GeneralLogger() { AddLogEntry = (logItem) => { } };
        }
    }
}
