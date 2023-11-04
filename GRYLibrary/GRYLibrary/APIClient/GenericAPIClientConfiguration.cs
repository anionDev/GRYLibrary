
using System.Net.Http;

namespace GRYLibrary.Core.GenericAPIClient
{
    public class GenericAPIClientConfiguration : IGenericAPIClientConfiguration
    {
        public string APIKey { get; set; }
        public string APIAddress { get; set; }
        /// <summary>
        /// This is an optional value to be able to pass a route to check if the server is available.
        /// </summary>
        /// <remarks>
        /// The route must be available using <see cref="HttpMethod.Get"/>.
        /// </remarks>
        public string TestRoute { get; set; }
        /// <inheritdoc/>
        public HttpClientHandler HttpClientHandler { get; set; }
    }
}
