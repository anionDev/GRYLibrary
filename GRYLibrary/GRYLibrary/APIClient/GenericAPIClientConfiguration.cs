
using System.Net.Http;

namespace GRYLibrary.Core.GenericAPIClient
{
    public class GenericAPIClientConfiguration : IGenericAPIClientConfiguration
    {
        public string APIKey { get; set; }
        public string APIAddress { get; set; }
        /// <inheritdoc/>
        public HttpClientHandler HttpClientHandler { get; set; }
    }
}
