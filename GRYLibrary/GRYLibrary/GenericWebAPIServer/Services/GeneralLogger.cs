using GRYLibrary.Core.Log;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using System;
using System.Dynamic;
using System.IO;
using System.Reflection;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class GeneralLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GeneralLogger Create(GRYLogConfiguration configuration)
        {
            var logObject =  GRYLog.Create(configuration);
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
