using System.Globalization;
using System.Net.Http;
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

        public async Task<decimal> GetAsDecimalAsync(string route)
        {
            return decimal.Parse(await this.GetAsStringAsync(route), CultureInfo.InvariantCulture);
        }
        public async Task<string> GetAsStringAsync(string route)
        {
            using HttpClient httpClient = this.GetHTTPClient();
            HttpResponseMessage response = await httpClient.GetAsync($"{this.Configuration.APIAddress}/{route}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        private HttpClient GetHTTPClient()
        {
            HttpClient result = new HttpClient();
            //TODO set apikey
            return result;
        }
    }
}
