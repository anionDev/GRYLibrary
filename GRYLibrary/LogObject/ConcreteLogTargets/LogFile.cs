using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.LogObject.ConcreteLogTargets
{
    public sealed class LogFile : GRYLogTarget
    {
        public LogFile() { }
        public string File { get; set; }
        public string Encoding { get; set; } = _UTF8Identifier;
        private const string _UTF8Identifier = "utf-8";
        private readonly IList<string> _Pool = new List<string>();
        public int PreFlushPoolSize { get; set; } = 1;
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            logItem.Format(logObject.Configuration, out string formattedMessage, out int _, out int _, out ConsoleColor _, this.Format, logItem.MessageId);
            this._Pool.Add(formattedMessage);
            if (this.PreFlushPoolSize <= this._Pool.Count)
            {
                this.Flush();
            }
        }
        public override void Dispose()
        {
            this.Flush();
        }

        public void Flush()
        {
            if (string.IsNullOrWhiteSpace(this.File))
            {
                throw new NullReferenceException($"LogFile is not defined");
            }
            string file = Utilities.ResolveToFullPath(this.File);
            Utilities.EnsureFileExists(file, true);
            string result = string.Empty;
            for (int i = 0; i < this._Pool.Count; i++)
            {
                if (!(i == 0 && Utilities.FileIsEmpty(file)))
                {
                    result += Environment.NewLine;
                }
                result += this._Pool[i];
            }
            Encoding encoding;
            if (this.Encoding.Equals(_UTF8Identifier))
            {
                encoding = new UTF8Encoding(false);
            }
            else
            {
                encoding = System.Text.Encoding.GetEncoding(this.Encoding);
            }
            System.IO.File.AppendAllText(file, result, encoding);
            this._Pool.Clear();
        }

        public override ISet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }
    }
}
