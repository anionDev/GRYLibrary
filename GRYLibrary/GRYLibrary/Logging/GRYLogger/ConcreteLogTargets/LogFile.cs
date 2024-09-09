using GRYLibrary.Core.Misc;
using GRYLibrary.Core.Misc.FilePath;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.Logging.GRYLogger.ConcreteLogTargets
{
    public sealed class LogFile : GRYLogTarget
    {
        public LogFile() { }
        public AbstractFilePath File { get; set; }
        public string Encoding { get; set; } = "utf-8";
        private readonly IList<string> _Pool = new List<string>();
        public int PreFlushPoolSize { get; set; } = 1;
        private string _BasePath;
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            this._BasePath = logObject.BasePath;
            logItem.Format(logObject.Configuration, out string formattedMessage, out int _, out int _, out ConsoleColor _, this.Format, logItem.MessageId);
            this._Pool.Add(formattedMessage);
            if (this.PreFlushPoolSize <= this._Pool.Count)
            {
                this.Flush();
            }
        }

        public void Flush()
        {
            string logfile = this.File.GetPath(this._BasePath);
            if (string.IsNullOrWhiteSpace(logfile))
            {
                throw new NullReferenceException($"LogFile is not defined.");
            }
            Utilities.EnsureFileExists(logfile, true);
            string result = string.Empty;
            for (int i = 0; i < this._Pool.Count; i++)
            {
                if (!(i == 0 && Utilities.FileIsEmpty(logfile)))
                {
                    result += Environment.NewLine;
                }
                result += this._Pool[i];
            }
            Encoding encoding = Utilities.GetEncodingByIdentifier(this.Encoding);
            System.IO.File.AppendAllText(logfile, result, encoding);
            this._Pool.Clear();
        }

        public override HashSet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization() => new HashSet<Type>();

        public override void Dispose() => this.Flush();

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}