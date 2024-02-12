using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IHTTPCredentialsProvider: ICredentialsProvider
    {
        public bool ContainsCredentials(HttpContext context);
        public string ExtractSecret(HttpContext context);
    }
}
