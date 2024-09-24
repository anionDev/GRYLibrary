using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.Logging.GRYLogger
{
    public interface IGRYLog : IDisposable, ILogger, IGeneralLogger
    {
        public GRYLogConfiguration Configuration { get; set; }
        public string BasePath { get; set; }
        public void Log(LogItem logitem);
        public IDisposable UseSubNamespace(string loggerName);
    }
}
