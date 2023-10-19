using System.Net.Http;

namespace GRYLibrary.Core.GenericAPIClient
{
    public interface IGenericAPIClientConfiguration
    {
        public string APIKey { get; set; }
        public string APIAddress { get; set; }
        /// <remarks>
        /// See https://stackoverflow.com/a/30605900/3905529 for proxy-usage-hints.
        /// </remarks>
        public HttpClientHandler HttpClientHandler { get; set; }
    }
}
