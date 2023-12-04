using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.Utilities;
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
        public void EnsureUserIsInGroup(string username, string groupname);
        public void EnsureUserIsNotInGroup(string username, string groupname);
        public bool UserIsInGroup(string username, string groupname);
        public bool GroupExists(string groupname);
        void OnStart();
        void RemoveUser(string username);
        bool IsAuthenticated(HttpContext context, AuthorizeAttribute authorizeAttribute);
        bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal);
    }
}
