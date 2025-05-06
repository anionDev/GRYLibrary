using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.Logging.GRYLogger
{
    public interface IGRYLog : IDisposable, ILogger, IGeneralLogger
    {
        public IGRYLogConfiguration Configuration { get; set; }
        public string BasePath { get; set; }
        public void Log(LogItem logitem);
        public void LogProgramOutput(string message, string[] stdOutLines,string[] stdErrLines,LogLevel logevel);
        public IDisposable UseSubNamespace(string loggerName);

        public void Log(string message, string messagId = null);
        public void Log(GRYLogTarget logTarget ,string message, LogLevel logLevel);
        public void Log(string message, Exception exception, string messageId = null);
        public void Log(string message, LogLevel logLevel, Exception exception, string messageId);
        public void Log(string message, LogLevel logLevel, string messageId = null);
        public void Log(Func<string> getMessage, string messageId = null);
        public void Log(Func<string> getMessage, Exception exception, string messageId = null);
        public void Log(Exception exception, string messageId = null);
        public void Log(LogLevel logLevel, Exception exception, string messageId = null);
        public void Log(Func<string> getMessage, LogLevel logLevel, Exception exception, string messageId = null);
        public void Log(Func<string> getMessage, LogLevel logLevel, string messageId = null);
    }
}
