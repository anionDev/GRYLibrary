using System.Collections.Generic;
using System;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using GRYLibrary.Core.APIServer.CommonDBTypes;

namespace GRYLibrary.Core.APIServer.Services.OpenIDConnect
{
    public class OpenIDConnectService : OpenIDConnectService<User>
    {
        public OpenIDConnectService(IOpenIDConnectServiceSettings settings, IGeneralLogger logger) : base(settings, logger)
        {
        }
    }
    public class OpenIDConnectService<UserType> : ExternalService, IOpenIDConnectService<UserType>
        where UserType : User
    {
        public IOpenIDConnectServiceSettings Settings { get; }
        public OpenIDConnectService(IOpenIDConnectServiceSettings settings, IGeneralLogger logger) : base(nameof(OpenIDConnectService), logger)
        {
            this.Settings = settings;
            this.Initialize();
        }

        private void Initialize()
        {
            this.TryConnect(out Exception _);
        }

        public bool UserIsInGroup(string name, string group)
        {
            throw new NotImplementedException();
        }

        public string Hash(string password)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public AccessToken Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public bool AccessTokenIsValid(string accessToken)
        {

            return this.EnsureServiceIsConnected<bool>(() =>
            {
                //example:
                string token = "eyJ..."; 
                string authority = "https://identitymanagement.example.de/realms/dein-realm";
                string clientId = "my-client";

                var user = ValidateJwtToken(token, authority, clientId);

                if (user != null)
                {
                    Console.WriteLine($"Benutzer angemeldet: {user.Identity.Name}");
                    return true;
                }
                else
                {
                    Console.WriteLine("Ungültiges Token");
                    return false;
                }


            });
        }
        private ClaimsPrincipal? ValidateJwtToken(string token, string authority, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidAudience = audience,
                ValidIssuer = authority,

                IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                {
                    var client = new HttpClient();
                    var jwks = client.GetFromJsonAsync<JsonWebKeySet>($"{authority}/.well-known/openid-configuration/jwks").Result;
                    return jwks.Keys;
                }
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (SecurityTokenException)
            {
                // Token ungültig
                return null;
            }
        }

        public string GetUserName(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(string userId)
        {
            throw new NotImplementedException();
        }

        public bool UserExists(string userId)
        {
            throw new NotImplementedException();
        }

        public ISet<User> GetAllUser()
        {
            throw new NotImplementedException();
        }

        public User GetUser(string userId)
        {
            throw new NotImplementedException();
        }

        public void AddRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public void EnsureUserHasRole(string userId, string roleId)
        {
            throw new NotImplementedException();
        }

        public void EnsureUserDoesNotHaveRole(string userId, string roleId)
        {
            throw new NotImplementedException();
        }

        public bool UserHasRole(string userId, string roleId)
        {
            throw new NotImplementedException();
        }

        public bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public void EnsureRoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public void EnsureRoleDoesNotExist(string roleName)
        {
            throw new NotImplementedException();
        }

        public Role GetRoleByName(string roleName)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetRolesOfUser(string userId)
        {
            throw new NotImplementedException();
        }

        public void Logout(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void Logout(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public void LogoutEverywhere(string userId)
        {
            throw new NotImplementedException();
        }

        public User GetUserByName(string name)
        {
            throw new NotImplementedException();
        }

        public User GetUserByAccessToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public bool UserExistsByName(string userNameAdmin)
        {
            throw new NotImplementedException();
        }

        public void UpdateRole(Role role)
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            throw new NotImplementedException();
        }

        public override bool TryConnectImplementation(out Exception exception)
        {
            throw new NotImplementedException();
        }

        public override void DisconnectImplementation()
        {
            throw new NotImplementedException();
        }

        public string GetBaseRoleOfAllUser()
        {
            throw new NotImplementedException();
        }
    }
}

