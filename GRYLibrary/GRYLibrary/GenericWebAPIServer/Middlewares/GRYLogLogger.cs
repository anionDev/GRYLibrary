﻿using GRYLibrary.Core.Log;
using System;
using System.IO;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public class GRYLogLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GRYLogLogger Create(string appName, string logFolder)
        {
            Miscellaneous.Utilities.EnsureDirectoryExists(logFolder);
            var logObject = GRYLog.Create();
            var initialConfiguration = new GRYLogConfiguration();
            string logFile = Path.Combine(logFolder, $"{appName}.log");
            initialConfiguration.ResetToDefaultValues(logFile);
            logObject.Configuration = Miscellaneous.Utilities.CreateOrLoadLoadXMLConfigurationFile("LogSettings.xml", initialConfiguration);
            return new GRYLogLogger()
            {
                AddLogEntry = (logEntry) =>
                {
                    logObject.Log(logEntry);
                }
            };
        }
    }
}
