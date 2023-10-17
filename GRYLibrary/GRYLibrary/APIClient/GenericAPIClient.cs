using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericAPIClient
{
    public class GenericAPIClient : IGenericAPIClient
    {
        public IGenericAPIClientConfiguration Configuration { get; set; }
        public GenericAPIClient(IGenericAPIClientConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        protected HttpClient GetHTTPClient()
        {
            HttpClient httpClient;
            if (this.Configuration.HttpClientHandler == null)
            {
                httpClient = new HttpClient();
            }
            else
            {
                httpClient = new HttpClient(handler: this.Configuration.HttpClientHandler);
            }
            return httpClient;
        }
        public async Task<decimal> GetAsDecimalAsync(string route)
        {
            return decimal.Parse(await this.GetAsStringAsync(route), CultureInfo.InvariantCulture);
        }
        public async Task<string> GetAsStringAsync(string route)
        {
            return await this.SendAsStringAsync(route, HttpMethod.Get);
        }
        public async Task<string> SendAsStringAsync(string route, HttpMethod method)
        {
            using HttpClient client = this.GetHTTPClient();
            HttpResponseMessage response;
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(method, $"{this.Configuration.APIAddress}/{route}"))
            {
                if (this.Configuration.APIKey != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("APIKey", this.Configuration.APIKey);
                }
                response = await client.SendAsync(requestMessage);
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public virtual void Dispose()
        {
            Miscellaneous.Utilities.NoOperation();
        }
    }
}
