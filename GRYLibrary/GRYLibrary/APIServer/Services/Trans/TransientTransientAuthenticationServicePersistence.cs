using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    public abstract class TransientTransientAuthenticationServicePersistence<UserType> : ITransientAuthenticationServicePersistence<UserType>
        where UserType : User
    {
        private IDictionary<string/*roleId*/, Role> _Roles;
        private IDictionary<string/*userId*/, UserType> _Users;
        public TransientTransientAuthenticationServicePersistence()
        {
            this._Roles = new Dictionary<string, Role>();
            this._Users = new Dictionary<string, UserType>();
        }

        public void SetAllUsers(ISet<UserType> users)
        {
            this._Users = users.ToDictionary(kvp => kvp.Id);
        }

        public IDictionary<string, Role> GetAlRoles()
        {
            return this._Roles.ToDictionary();
        }

        public void SetAllRoles(ISet<Role> roles)
        {
            this._Roles = roles.ToDictionary(kvp => kvp.Id);
        }

        public IDictionary<string, UserType> GetAllUsers()
        {
            return this._Users.ToDictionary();
        }

        public bool AccessTokenExists(string accessToken, out User user)
        {
            UserType result= this.GetAllUsers().Values.Where(u => u.AccessToken.Where(at => at.Value == accessToken).Any()).FirstOrDefault();
            if(result == default)
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
            return this._Users.Values.Where(u=>u.Name== userName).Any();
        }
        public bool UserWithIdExists(string userId)
        {
            return this._Users.ContainsKey(userId);
        }

        public UserType GetuserById(string userId)
        {
            return this._Users[userId];
        }

        public UserType GetuserByName(string userName)
        {
            return this._Users.Values.Where(u => u.Name == userName).First();
        }

        public UserType GetUserById(object userId)
        {
            throw new NotImplementedException();
        }

        public UserType GetUserByName(object userName)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(UserType user)
        {
            throw new NotImplementedException();
        }
    }
}
