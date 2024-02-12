using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    /// <summary>
    /// This is a transient <see cref="IAuthenticationService"/> for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement any features to increase security.
    /// </remarks>
    public class TransientAuthenticationService<UserType> : IAuthenticationService
        where UserType : User
    {
        private readonly ITimeService _TimeService;
        private readonly IUserCreatorService<UserType> _UserCreatorService;
        private readonly IDictionary<string/*username*/, UserType> _Users;
        public TransientAuthenticationService(ITimeService timeService, IUserCreatorService<UserType> userCreatorService)
        {
            this._TimeService = timeService;
            this._UserCreatorService = userCreatorService;
            this._Users = new Dictionary<string, UserType>();
        }
        public string Hash(string input)
        {
            throw new NotImplementedException();
        }
        private UserType GetUserByName(string username)
        {
            if (this._Users.TryGetValue(username, out UserType value))
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
                throw new BadRequestException((int)System.Net.HttpStatusCode.BadRequest, "User does not exist.");
            }
            string passwordHashsed = this.Hash(password);
            UserType user = this.GetUserByName(username);
            if (passwordHashsed == user.PasswordHash)
            {
                AccessToken newAccessToken = new AccessToken();
                newAccessToken.Value = Guid.NewGuid().ToString();
                newAccessToken.ExpiredMoment = this._TimeService.GetCurrentTime().AddDays(1);//this time should be moved to IKeyCloakServiceSettings if it is implementable in the real keycloack-service too.
                user.AccessToken.Add(newAccessToken);
                return newAccessToken;
            }
            else
            {
                throw new BadRequestException((int)System.Net.HttpStatusCode.Unauthorized, "Invalid password.");
            }
        }

        public void Register(string username, string password)
        {
            if (this.UserExists(username))
            {
                throw new BadRequestException((int)System.Net.HttpStatusCode.BadRequest, "User with this name already exists.");
            }
            string passwordHashsed = this.Hash(password);
            UserType user = this._UserCreatorService.CreateUser(username, passwordHashsed);
            this._Users.Add(user.Id, user);
        }

        public void Logout(AccessToken accessToken)
        {
            this._Users[this.GetUserName(accessToken.Value)].AccessToken.Remove(accessToken);
        }

        public virtual void OnStart()
        {
        }

        public void RemoveUser(string username)
        {
            this._Users.Remove(username);
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

        public bool UserExists(string username)
        {
            return this._Users.ContainsKey(username);
        }

        public bool AccessTokenIsValid(string accessToken)
        {
            foreach (KeyValuePair<string, UserType> user in this._Users)
            {
                foreach (AccessToken at in user.Value.AccessToken)
                {
                    if (at.Value == accessToken)
                    {
                        return this._TimeService.GetCurrentTime() < at.ExpiredMoment;
                    }
                }
            }
            return false;
        }
        public string GetUserName(string accessToken)
        {
            foreach (KeyValuePair<string, UserType> user in this._Users)
            {
                foreach (AccessToken at in user.Value.AccessToken)
                {
                    if (at.Value == accessToken)
                    {
                        return user.Key;
                    }
                }
            }
            throw new KeyNotFoundException();
        }

        public string GetIdOfUser(string username)
        {
            return this._Users.Values.Where(user => user.Name == username).First().Id;
        }
    }
}
