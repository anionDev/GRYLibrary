using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.IO;
using System.Linq;

namespace GRYLibrary.Core.Logging.GeneralPurposeLogger
{
    public class GeneralLogger : IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
        public static GeneralLogger NoLog()
        {
            return new GeneralLogger() { AddLogEntry = (logItem) => { } };
        }

        public static IGRYLog CreateUsingGRYLog(GRYLogConfiguration configuration)
        {
            return CreateUsingGRYLog(configuration, Directory.GetCurrentDirectory());
        }

        public static IGRYLog CreateUsingGRYLog(GRYLogConfiguration configuration, string basePath = null)
        {
            GRYLog logObject = GRYLog.Create(configuration);
            logObject.BasePath = basePath;
            return logObject;
        }

        public static IGRYLog NoLogAsGRYLog()
        {
            GRYLog logObject = GRYLog.Create();
            logObject.Configuration.LogTargets.Clear();
            return logObject;
        }


        public static IGRYLog CreateUsingConsole()
        {
            GRYLog logObject = GRYLog.Create();
            logObject.Configuration.LogTargets = logObject.Configuration.LogTargets.Where(target => target is GRYLogger.ConcreteLogTargets.Console).ToList();
            return logObject;
        }
    }
}