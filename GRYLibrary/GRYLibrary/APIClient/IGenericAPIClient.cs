using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericAPIClient
{
    public interface IGenericAPIClient:IDisposable
    {
        public IGenericAPIClientConfiguration Configuration { get; set; }
        public Task<decimal> GetAsDecimalAsync(string route);
        public Task<string> GetAsStringAsync(string route);
    }
}
