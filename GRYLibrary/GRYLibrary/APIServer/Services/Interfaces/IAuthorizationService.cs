using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IAuthorizationService
    {
        public bool IsAuthorized(string action, string secret);
        bool IsAuthorized(HttpContext context, AuthorizeAttribute authorizedAttribute);
    }
}
