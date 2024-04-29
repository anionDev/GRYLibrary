using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Crypto;
using GRYLibrary.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using AccessToken = GRYLibrary.Core.APIServer.CommonAuthenticationTypes.AccessToken;
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

        private readonly IDictionary<string/*groupname*/, UserGroup> _Groups;
        private readonly ITimeService _TimeService;
        private readonly IUserCreatorService _UserCreatorService;
        private readonly IDictionary<string/*username*/, User> _Users;
        public TransientAuthenticationService(ITimeService timeService, IUserCreatorService userCreatorService)
        {
            this._Groups = new Dictionary<string, UserGroup>();
            this._TimeService = timeService;
            this._UserCreatorService = userCreatorService;
            this._Users = new Dictionary<string, User>();
        }

        public string Hash(string password)
        {
            string result = GUtilities.ByteArrayToHexString(new SHA256().Hash(GUtilities.StringToByteArray(password)));
            return result;
        }

        private User GetUserByName(string username)
        {
            if (this._Users.TryGetValue(username, out User value))
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
            User user = this.GetUserByName(username);
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
            User user = this._UserCreatorService.CreateUser(username, passwordHashsed);
            this._Users.Add(user.Name, user);
        }

        public void Logout(AccessToken accessToken)
        {
            this._Users[this.GetUserName(accessToken.Value)].AccessToken.Remove(accessToken);
        }

        public void LogoutEverywhere(string username)
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


        public bool UserExists(string username)
        {
            return this._Users.ContainsKey(username);
        }

        public bool AccessTokenIsValid(string accessToken)
        {
            foreach (KeyValuePair<string, User> user in this._Users)
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
            foreach (KeyValuePair<string, User> user in this._Users)
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

        public void EnsureUserIsInGroup(string username, string groupname)
        {
            string userId = this.GetIdOfUser(username);
            this._Groups[groupname].UserIds.Add(userId);
        }

        public void EnsureUserIsNotInGroup(string username, string groupname)
        {
            string userId = this.GetIdOfUser(username);
            this._Groups[groupname].UserIds.Remove(userId);
        }

        public bool UserIsInGroup(string username, string groupname)
        {
            string userId = this.GetIdOfUser(username);
            return this._Groups[groupname].UserIds.Contains(userId);
        }

        public void EnsureGroupExists(string groupname)
        {
            if (!this._Groups.ContainsKey(groupname))
            {
                UserGroup group = new UserGroup();
                group.Name = groupname;
                this._Groups[groupname] = group;
            }
        }

        public void EnsureGroupDoesNotExist(string groupname)
        {
            if (this._Groups.ContainsKey(groupname))
            {
                this._Groups.Remove(groupname);
            }
        }
        public bool GroupExists(string groupname)
        {
            return this._Groups.ContainsKey(groupname);
        }


        public ISet<string> GetGroupsOfUser(string username)
        {
            throw new NotImplementedException();
        }

        public ISet<User> GetAllUser()
        {
            return this._Users.Values.ToHashSet();
        }
    }
}
