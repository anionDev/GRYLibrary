using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.KeyCloak;
using GRYLibrary.Core.Crypto;
using System.Collections.Generic;
using System.Security.Claims;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Services.KCZAuth
{
    public class KeyCloakAuthenticationService : IKeyCloakAuthenticationService
    {
        private readonly IKeyCloakService _KeyCloakService;
        private readonly IHTTPCredentialsProvider _HTTPCredentialsProvider;
        public KeyCloakAuthenticationService(IKeyCloakService keyCloakService, IHTTPCredentialsProvider httpCredentialsProvider)
        {
            this._KeyCloakService = keyCloakService;
            this._HTTPCredentialsProvider = httpCredentialsProvider;
        }

        public bool AccessTokenIsValid(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureRoleDoesNotExist(string groupname)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureRoleExists(string groupUser)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureUserHasRole(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureUserDoesNotHaveRole(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }

        public ISet<User> GetAllUser()
        {
            throw new System.NotImplementedException();
        }

        public ISet<string> GetRolesOfUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public string GetIdOfUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public User GetUser(string userId)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserName(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public bool RoleExists(string groupname)
        {
            throw new System.NotImplementedException();
        }

        public string Hash(string password)
        {
            return GUtilities.ByteArrayToHexString(new SHA256().Hash(GUtilities.StringToByteArray(password)));
        }

        public AccessToken Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }
        public void LogoutEverywhere(string username)
        {
            throw new System.NotImplementedException();
        }

        public void Register(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public bool UserExists(string username)
        {
            throw new System.NotImplementedException();
        }

        public bool UserHasRole(string username, string groupname)
        {
            throw new System.NotImplementedException();
        }

        public void AddUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public Role GetRoleByName(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public User GetUserByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public User GetUserByAccessToken(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void AddRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public bool UserExistsByName(string userNameAdmin)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateRole(Role role)
        {
            throw new System.NotImplementedException();
        }

        public void Logout(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void Logout(ClaimsPrincipal user)
        {
            throw new System.NotImplementedException();
        }

        public ISet<Role> GetRoles(ClaimsPrincipal user)
        {
            throw new System.NotImplementedException();
        }
    }
}
