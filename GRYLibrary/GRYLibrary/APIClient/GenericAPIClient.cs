using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericAPIClient
{
    public class GenericAPIClient : IGenericAPIClient
    {
        private readonly HttpClient _HTTPClient;
        public IGenericAPIClientConfiguration Configuration { get; set; }
        public GenericAPIClient(IGenericAPIClientConfiguration configuration)
        {
            this.Configuration = configuration;
            if (this.Configuration.HttpClientHandler == null)
            {
                this._HTTPClient = new HttpClient();
            }
            else
            {
                this._HTTPClient = new HttpClient(handler: this.Configuration.HttpClientHandler);
            }
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
            HttpResponseMessage response;
            using (var requestMessage = new HttpRequestMessage(method, $"{this.Configuration.APIAddress}/{route}"))
            {
                if (this.Configuration.APIKey != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("APIKey", this.Configuration.APIKey);
                }
                response = await this._HTTPClient.SendAsync(requestMessage);
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            this._HTTPClient.Dispose();
        }
    }
}
