using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    /// <summary>
    /// This is a transient <see cref="IAuthenticationService"/> for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement any features to increase security.
    /// </remarks>
    public class TransientAuthenticationService : IAuthenticationService
    {
        private readonly ITimeService _TimeService;
        private readonly IDictionary<string/*username*/, UserBackendInformation> _Users;
        public TransientAuthenticationService(ITimeService timeService)
        {
            this._TimeService = timeService;
            this._Users = new Dictionary<string, UserBackendInformation>();
        }
        public bool AccessTokenIsValid(string username, string accessToken)
        {
            try
            {
                UserBackendInformation user = this.GetUserByName(username);
                AccessToken token = user.AccessToken[accessToken];
                return this._TimeService.GetCurrentTime() < token.ExpiredMoment;
            }
            catch
            {
                return false;
            }
        }

        private UserBackendInformation GetUserByName(string username)
        {
            if (this._Users.TryGetValue(username, out UserBackendInformation value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"No user found with username {username}.");
            }
        }


        public AccessToken Login(string username, string password)
        {
            if (!this.UserExists(username))
            {
                throw new BadRequestException(System.Net.HttpStatusCode.BadRequest, "User does not exist.");
            }
            UserBackendInformation user = this.GetUserByName(username);
            if (password == user.Password)
            {
                AccessToken newAccessToken = new AccessToken();
                newAccessToken.Value = Guid.NewGuid().ToString();
                newAccessToken.ExpiredMoment = this._TimeService.GetCurrentTime().AddDays(1);//this time should be moved to IKeyCloakServiceSettings if it is implementable in the real keycloack-service too.
                user.AccessToken[newAccessToken.Value] = newAccessToken;
                return newAccessToken;
            }
            else
            {
                throw new BadRequestException(System.Net.HttpStatusCode.Unauthorized, "Invalid password.");
            }
        }

        public void Register(string username, string password)
        {
            if (this.UserExists(username))
            {
                throw new BadRequestException(System.Net.HttpStatusCode.BadRequest, "User with this name already exists.");
            }
            UserBackendInformation userBackendInformation = new UserBackendInformation()
            {
                User = new User()
                {
                    Id = Guid.NewGuid(),
                    Name = username,
                },
                Password = password,
            };
            this._Users.Add(userBackendInformation.User.Name, userBackendInformation);
        }

        public void Logout(string username)
        {
            this._Users[username].AccessToken.Clear();
        }

        public virtual void OnStart()
        {
        }

        public void RemoveUser(string username)
        {
            this._Users.Remove(username);
        }

        public bool IsAuthenticated(HttpContext context, AuthorizeAttribute authorizeAttribute)
        {
            throw new NotImplementedException();
        }
        public static string CookieName { get; set; } = "X-Authorization";
        //TODO move CookieName to configuration
        //TODO remove "X-"-prefix from CookieName
        public static (string key, string value, CookieOptions options) GetAccessTokenCookie(string username, string accessToken, DateTime expires)
        {
            return (CookieName,
                $"User={username};AccessToken={accessToken}",
                new CookieOptions()
                {
                    Expires = expires,
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                }
            );
        }

        public static (string key, string value, CookieOptions options) GetAccessTokenExpiredCookie(string username)
        {
            return (CookieName,
                $"User={username};AccessToken=",
                new CookieOptions()
                {
                    Expires = new DateTime(1970, 1, 1, 0, 0, 0),
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                }
            );
        }

        public bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal)
        {
            principal = default;
            try
            {
                if (context.Request.Cookies.TryGetValue(CookieName, out string value))
                {
                    string[] splitted = value.Split(';');
                    string username = splitted[0].Split('=')[1];
                    string accessToken = splitted[1].Split('=')[1];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        bool accessTokenIsValid = AccessTokenIsValid(username, accessToken);
                        if (accessTokenIsValid)
                        {
                            principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, username) }, "Basic"));
                            return true;
                        }
                    }
                }
            }
            catch
            {
                GUtilities.NoOperation();
            }
            return false;
        }

        UserBackendInformation IAuthenticationService.GetUserByName(string username)
        {
            return this._Users[username];
        }

        public bool UserExists(string username)
        {
            return this._Users.ContainsKey(username);
        }
    }
}
