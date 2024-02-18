using GRYLibrary.Core.APIServer.Services.CredC;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Services.Cred
{
    public class ExtractFromCookieService : IExtractFromCookieService
    {
        public IExtractFromCookieServiceConfiguration ExtractFromCookieServiceConfiguration { get; set; }
        public ExtractFromCookieService(IExtractFromCookieServiceConfiguration extractFromCookieServiceConfiguration)
        {
            this.ExtractFromCookieServiceConfiguration = extractFromCookieServiceConfiguration;
        }
        public virtual string ExtractSecret(HttpContext context)
        {
            this.TryGetCookieValue(context, out string result);
            return result;
        }

        public virtual bool ContainsCredentials(HttpContext context)
        {
            return this.TryGetCookieValue(context, out string _);
        }
        protected virtual bool TryGetCookieValue(HttpContext context, out string cookie)
        {
            return context.Request.Cookies.TryGetValue(this.ExtractFromCookieServiceConfiguration.CookieName, out cookie);
        }
    }
}
