using GRYLibrary.Core.Log;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public class GRYLogLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GRYLogLogger Create(string appName, string logFolder)
        {
            Core.Miscellaneous.Utilities.EnsureDirectoryExists(logFolder);
            var logObject = GRYLog.Create();
            logObject.Configuration = Miscellaneous.Utilities.CreateOrLoadLoadXMLConfigurationFile("LogSettings.xml", new GRYLogConfiguration());
            //TODO configure file-target to use logfolder. in the end console- and filetarget should be enabled, no else targets
            return new GRYLogLogger()
            {
                AddLogEntry = (logEntry) => { logObject.Log(logEntry); }
            };
        }
    }
}
