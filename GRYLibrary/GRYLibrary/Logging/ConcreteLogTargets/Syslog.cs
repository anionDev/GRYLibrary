using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.ExecutePrograms;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public sealed class Syslog : GRYLogTarget
    {

        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            string messageId;
            if (string.IsNullOrWhiteSpace(logItem.MessageId))
            {
                messageId = string.Empty;
            }
            else
            {
                messageId = $"--rfc5424 --msgid {logItem.MessageId}";
            }

            using ExternalProgramExecutor externalProgramExecutor = new("Logger", $"--tag {Utilities.GetNameOfCurrentExecutable()} {messageId} -- [{logItem.LogLevel}] [{logObject.Configuration.Name}] {logItem.PlainMessage}");
            externalProgramExecutor.Run();
        }

        public override HashSet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }
        public override void Dispose()
        {
            Utilities.NoOperation();
        }
    }
}
