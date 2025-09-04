using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Services.Aut.Prov
{
    public interface IAuthenticationProvider
    {
        public bool IsApplicable(HttpContext context);
        public bool TryGetAuthentication(HttpContext context, out string accessToken);
    }
}
