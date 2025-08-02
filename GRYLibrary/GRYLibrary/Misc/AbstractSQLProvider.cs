using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace GRYLibrary.Core.Misc
{
    public class AbstractSQLProvider
    {
        private readonly string _SQLFilesNamespace;
        private bool _Verbose;
        private IGRYLog _Log;
        protected AbstractSQLProvider(string sqlFilesNamespace, IGRYLog log)
        {
            this._SQLFilesNamespace = sqlFilesNamespace;
            this._Log = log;
        }
        private readonly IDictionary<string, string> _ScriptCache = new Dictionary<string, string>();

        protected string LoadSQLScript(string sqlFileName)
        {
            this._Log.Log($"Load SQL-script \"{sqlFileName}\"", LogLevel.Trace);
            if (!this._ScriptCache.ContainsKey(sqlFileName))
            {
                this.LoadScriptToCache(sqlFileName);
            }
            return this._ScriptCache[sqlFileName];
        }

        private void LoadScriptToCache(string sqlFileName)
        {
            using Stream stream = this.GetType().Assembly.GetManifestResourceStream($"{this._SQLFilesNamespace}.{sqlFileName}.sql");
            using StreamReader reader = new StreamReader(stream);
            this._ScriptCache[sqlFileName] = reader.ReadToEnd();
        }
    }
}
