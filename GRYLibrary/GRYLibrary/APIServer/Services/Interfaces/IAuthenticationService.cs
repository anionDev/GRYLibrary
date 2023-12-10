using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public void Register(string username, string password);
        public AccessToken Login(string username, string password);
        /// <summary>
        /// Checks if the <paramref name="accessToken"/> is a valid authentication for <paramref name="username"/>.
        /// </summary>
        public bool AccessTokenIsValid(string username, string accessToken);
        void Logout(string name);
        public void RemoveUser(string username);
        public  bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal);
        UserBackendInformation GetUserByName(string username);
        bool UserExists(string username);
    }
}
