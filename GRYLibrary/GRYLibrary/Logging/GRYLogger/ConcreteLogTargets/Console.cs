using GRYLibrary.Core.Misc;
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
            //TODO refactor to do this in one write-statement by using the codes described in https://stackoverflow.com/a/74807043/3905529

            string part1 = formattedMessage.AsSpan(0, cb).ToString();
            output.Write(part1);

            string part2 = formattedMessage[cb..ce];
            this.WriteWithColorToConsole(part2, output, logItem.LogLevel, logObject);

            string part3 = formattedMessage[ce..] + Environment.NewLine;
            output.Write(part3);

            output.Flush();
        }
        public override HashSet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization() => new HashSet<Type>() { typeof(ConsoleColor) };
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

        public override void Dispose() => Utilities.NoOperation();
        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}