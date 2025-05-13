using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GRYLibrary.Core.APIServer.Services.Res
{
    public class GeneralResourceLoader : IGeneralResourceLoader
    {
        private readonly IDictionary<string, byte[]> _Cache = new Dictionary<string, byte[]>();
        private readonly string _BaseNamespace;
        protected readonly Assembly _Assembly;
        public GeneralResourceLoader(string baseNamespace, Assembly assembly)
        {
            this._BaseNamespace = baseNamespace;
            this._Assembly = assembly;
        }
        public byte[] GetResource(string resourceName)
        {
            if (!this._Cache.TryGetValue(resourceName, out byte[]? value))
            {
                using Stream? resFilestream = this._Assembly.GetManifestResourceStream(this._BaseNamespace + "." + resourceName);
                if (resFilestream == null)
                {
                    throw new KeyNotFoundException($"No resource available with name \"{resourceName}\".");
                }
                else
                {
                    byte[] content = new byte[resFilestream.Length];
                    resFilestream.Read(content, 0, content.Length);
                    value = content;
                    this._Cache[resourceName] = value;
                }
            }
            return value;
        }
    }
}
