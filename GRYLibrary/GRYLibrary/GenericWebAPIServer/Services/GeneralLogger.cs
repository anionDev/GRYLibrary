using GRYLibrary.Core.Log;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class GeneralLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GeneralLogger Create(GRYLogConfiguration configuration)
        {
            GRYLog logObject =  GRYLog.Create(configuration);
            return new GeneralLogger()
            {
                AddLogEntry = logObject.Log
            };
        }

        public static GeneralLogger NoLog()
        {
            return new GeneralLogger() { AddLogEntry = (logItem) => { } };
        }
    }
}
