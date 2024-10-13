using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIClient
{
    public interface IGenericAPIClient : IDisposable
    {
        public IGenericAPIClientConfiguration Configuration { get; set; }
        public Task<decimal> GetAsDecimalAsync(string route);
        public Task<string> GetAsStringAsync(string route);
        public Task<bool> IsAvailable();
        Task PostAsync(string route, string body);
        Task PutAsync(string route, string body);
    }
}
