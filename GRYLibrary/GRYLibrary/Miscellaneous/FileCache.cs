using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GRYLibrary.Core.Miscellaneous
{
    public class FileCache<TKey, TValue>
        where TKey : ISimpleSerializable, new()
        where TValue : ISimpleSerializable, new()
    {
        public string CacheFile { get; set; }
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        private IDictionary<TKey, TValue> Cache = new Dictionary<TKey, TValue>();
        public FileCache(string cacheFile)
        {
            this.CacheFile = cacheFile;
        }
        public void Load()
        {
            foreach (string line in File.ReadLines(this.CacheFile, this.Encoding))
            {
                string[] splitted = line.Split(';');
                TKey tkey = new TKey();
                tkey.DeserializeFromString(splitted[0]);
                TValue tvalue = new TValue();
                tvalue.DeserializeFromString(splitted[1]);
                this.Cache[tkey] = tvalue;
            }
        }
        public bool Contains(TKey t)
        {
            return this.Cache.ContainsKey(t);
        }
        public TValue Get(TKey t)
        {
            return this.Cache[t];
        }
        public void Set(TKey t, TValue value)
        {
            this.Cache[t] = value;
            this.Save();
        }
        public void Remove(TKey t)
        {
            this.Cache.Remove(t);
            this.Save();
        }

        private void Save()
        {
            List<string> lines = new List<string>();
            foreach (KeyValuePair<TKey, TValue> kvp in this.Cache)
            {
                lines.Add($"{kvp.Key.SerializeToString()};{kvp.Value.SerializeToString()}");
            }
            File.WriteAllLines(this.CacheFile, lines);
        }
    }
}
