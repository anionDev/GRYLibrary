using System.Collections.Generic;
using System.IO;

namespace GRYLibrary.Core.APIServer.Services.Res
{
    public class GeneralResourceLoader : IGeneralResourceLoader
    {
        private readonly IDictionary<string, byte[]> _Cache = new Dictionary<string, byte[]>();
        private readonly string _BaseNamespace;
        public GeneralResourceLoader(string baseNamespace)
        {
            _BaseNamespace = baseNamespace;
        }
        public byte[] GetResource(string resourceName)
        {
            if (!_Cache.TryGetValue(resourceName, out byte[]? value))
            {
                System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                using Stream resFilestream = a.GetManifestResourceStream(_BaseNamespace + "." + resourceName);
                if (resFilestream == null)
                    return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                value = ba;
                _Cache[resourceName] = value;
            }
            return value;
        }
    }
}
