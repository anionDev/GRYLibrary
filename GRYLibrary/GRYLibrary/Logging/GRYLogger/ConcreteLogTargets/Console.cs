using GRYLibrary.Core.Miscellaneous;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace GRYLibrary.Core.Logging.GRYLogger.ConcreteLogTargets
{
    public sealed class Console : GRYLogTarget
    {
        public bool WriteWarningsToStdErr { get; set; } = true;
        public Console() { }
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            TextWriter output;
            if (logItem.IsErrorEntry() || this.WriteWarningsToStdErr && logItem.LogLevel == LogLevel.Warning)
            {
                output = System.Console.Error;
            }
            else
            {
                output = System.Console.Out;
            }
            logItem.Format(logObject.Configuration, out string formattedMessage, out int cb, out int ce, out ConsoleColor _, this.Format, logItem.MessageId);
            output.Write(formattedMessage.AsSpan(0, cb));
            this.WriteWithColorToConsole(formattedMessage[cb..ce], output, logItem.LogLevel, logObject);
            output.Write(formattedMessage[ce..] + Environment.NewLine);
        }
        public override HashSet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>() { typeof(ConsoleColor) };
        }
        private void WriteWithColorToConsole(string message, TextWriter output, LogLevel logLevel, GRYLog logObject)
        {
            if (message.Length > 0)
            {
                try
                {
                    System.Console.ForegroundColor = logObject.Configuration.GetLoggedMessageTypesConfigurationByLogLevel(logLevel).ConsoleColor;
                    output.Write(message);
                }
                finally
                {
                    System.Console.ForegroundColor = logObject._ConsoleDefaultColor;
                }
            }
        }

        public override void Dispose()
        {
            Utilities.NoOperation();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}