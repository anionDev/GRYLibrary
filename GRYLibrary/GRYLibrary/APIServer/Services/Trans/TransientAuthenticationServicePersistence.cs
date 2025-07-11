﻿using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
namespace GRYLibrary.Core.APIServer.Services.Trans
{
    public class TransientAuthenticationServicePersistence<UserType> : ITransientAuthenticationServicePersistence<UserType>
        where UserType : User
    {
        private readonly IDictionary<string/*roleId*/, Role> _Roles;
        private readonly IDictionary<string/*userId*/, UserType> _Users;
        private readonly IDictionary<string/*userId*/, ISet<AccessToken>> _AccessTokens;
        private readonly ITimeService _TimeService;
        public TransientAuthenticationServicePersistence(ITimeService timeService)
        {
            this._Roles = new Dictionary<string, Role>();
            this._Users = new Dictionary<string, UserType>();
            this._AccessTokens = new Dictionary<string, ISet<AccessToken>>();
            this._TimeService = timeService;
        }

        public void SetAllUsers(ISet<UserType> users)
        {
            this._Users.Clear();
            foreach (UserType user in users)
            {
                this._Users.Add(user.Id, user);
            }
        }

        public ISet<Role> GetAllRoles()
        {
            return this._Roles.Values.ToHashSet();
        }

        public void SetAllRoles(ISet<Role> roles)
        {
            this._Roles.Clear();
            foreach (Role role in roles)
            {
                this._Roles.Add(role.Id, role);
            }
        }

        public IDictionary<string, UserType> GetAllUsers()
        {
            return this._Users.ToDictionary();
        }

        public bool AccessTokenExists(string accessToken, out UserType? user)
        {
            UserType? result = this.GetAllUsers().Values.Where(u => u.AccessToken.Where(at => at.Value == accessToken).Any()).FirstOrDefault();
            if (result == default)
            {
                user = null;
                return false;
            }
            else
            {
                user = result;
                return true;
            }
        }

        public void AddUser(UserType newUser)
        {
            this._Users[newUser.Id] = newUser;
        }

        public bool UserWithNameExists(string userName)
        {
            return this._Users.Values.Where(u => u.Name == userName).Any();
        }

        public bool UserWithIdExists(string userId)
        {
            return this._Users.ContainsKey(userId);
        }

        public UserType GetuserById(string userId)
        {
            return this._Users[userId];
        }

        public void RemoveUser(string userId)
        {
            this._Users.Remove(userId);
        }

        public UserType GetUserById(string userId)
        {
            return this._Users[userId];
        }

        public UserType GetUserByName(string userName)
        {
            return this._Users.Values.Where(u => u.Name == userName).First();
        }

        public UserType GetUserByAccessToken(string accessToken)
        {
            foreach (UserType user in this._Users.Values)
            {
                IEnumerable<CommonAuthenticationTypes.AccessToken> tokens = user.AccessToken.Where(a => a.Value == accessToken);
                int tokenCount = tokens.Count();
                if (tokenCount == 0)
                {
                    continue;
                }
                GUtilities.AssertCondition(tokenCount == 1, "Internal error while checking access token");
                CommonAuthenticationTypes.AccessToken token = tokens.First();
                System.DateTime now = this._TimeService.GetCurrentTime();
                if (token.ExpiredMoment < now)
                {
                    throw new CredentialsExpiredException();
                }
                return user;
            }
            throw new KeyNotFoundException("No user found with given accesstoken.");
        }

        public bool RoleExists(string roleName)
        {
            return this._Roles.Where(kvp => kvp.Value.Name == roleName).Any();
        }

        public void AddRole(Role role)
        {
            this._Roles[role.Id] = role;
        }

        public void UpdateRole(Role role)
        {
            this._Roles[role.Id] = role;
        }

        public void DeleteRoleByName(string roleName)
        {
            foreach (UserType user in this._Users.Values)
            {
                if (this.UserHasRole(user.Id, roleName))
                {
                    this.RemoveRoleFromUser(user.Id, roleName);
                }
            }
            this._Roles.Remove(roleName);
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            this._Users[userId].Roles.Add(this._Roles[roleId]);
        }

        public bool UserHasRole(string userId, string roleId)
        {
            return this._Users[userId].Roles.Contains(this._Roles[roleId]);
        }

        public void RemoveRoleFromUser(string userId, string roleId)
        {
            this._Users[userId].Roles.Remove(this._Roles[roleId]);
        }

        public void UpdateUser(UserType user)
        {
            this._Users[user.Id] = user;
        }

        public AccessToken GetAccessToken(string accessToken)
        {
            foreach (KeyValuePair<string, ISet<AccessToken>> kvp in this._AccessTokens)
            {
                foreach (AccessToken token in kvp.Value)
                {
                    if (token.Value == accessToken)
                    {
                        return token;
                    }
                }
            }
            throw new KeyNotFoundException($"AccessToken '{accessToken}' is unknown.");
        }

        public void AddAccessToken(string userId, AccessToken newAccessToken)
        {
            if (!this._AccessTokens.TryGetValue(userId, out ISet<AccessToken>? value))
            {
                value = new HashSet<AccessToken>();
                this._AccessTokens[userId] = value;
            }

            value.Add(newAccessToken);
        }

        public void RemoveAccessToken(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public ISet<AccessToken> GetAllAccessTokenOfUser(string userId)
        {
            return this._AccessTokens[userId].ToHashSet();
        }

        public Role GetRoleByName(string roleName)
        {
            return _Roles.Values.Where(role => role.Name == roleName).First();
        }
    }
}
