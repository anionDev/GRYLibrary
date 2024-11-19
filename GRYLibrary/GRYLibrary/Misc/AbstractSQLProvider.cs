using System.Collections.Generic;
using System.IO;

namespace GRYLibrary.Core.Misc
{
    public class AbstractSQLProvider
    {
        private readonly string _SQLFilesNamespace;
        protected AbstractSQLProvider(string sqlFilesNamespace)
        {
            this._SQLFilesNamespace = sqlFilesNamespace;
        }
        private readonly IDictionary<string, string> _ScriptCache = new Dictionary<string, string>();

        protected string LoadSQLScript(string sqlFileName)
        {
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
