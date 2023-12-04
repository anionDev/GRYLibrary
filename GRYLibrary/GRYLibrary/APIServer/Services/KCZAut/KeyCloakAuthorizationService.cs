using GRYLibrary.Core.APIServer.Services.KeyCloak;
using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.APIServer.Services.KCZAut
{
    public class KeyCloakAuthorization : IKeyCloakAuthorizationService
    {
        private readonly IKeyCloakService _KeyCloakService;
        public KeyCloakAuthorization(IKeyCloakService keyCloakService)
        {
            _KeyCloakService = keyCloakService;
        }
        public bool IsAuthorized(HttpContext context, Utilities.AuthorizeAttribute authorizeAttribute)
        {
            System.Collections.Generic.ISet<string> groups = authorizeAttribute.Groups;
            foreach (string group in groups)
            {
                if (_KeyCloakService.UserIsInGroup(context.User.Identity.Name, group))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAuthorized(string action)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthorized(string action, string secret)
        {
            throw new System.NotImplementedException();
        }
    }
}
