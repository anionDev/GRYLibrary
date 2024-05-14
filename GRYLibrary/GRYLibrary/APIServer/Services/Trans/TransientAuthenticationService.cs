using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Crypto;
using GRYLibrary.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using AccessToken = GRYLibrary.Core.APIServer.CommonAuthenticationTypes.AccessToken;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using User = GRYLibrary.Core.APIServer.CommonDBTypes.User;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    /// <summary>
    /// This is a transient <see cref="IAuthenticationService{UserType}"/> for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement many features to increase security.
    /// </remarks>
    public class TransientAuthenticationService<UserType> : IAuthenticationService<UserType>
        where UserType : User
    {

        private readonly ITimeService _TimeService;
        private readonly ITransientAuthenticationServicePersistence<UserType> _TransientAuthenticationServicePersistence;
        public TransientAuthenticationService(ITimeService timeService, ITransientAuthenticationServicePersistence<UserType> transientAuthenticationServicePersistence)
        {
            this._TimeService = timeService;
            this._TransientAuthenticationServicePersistence = transientAuthenticationServicePersistence;
        }

        public string Hash(string password)
        {
            string result = GUtilities.ByteArrayToHexString(new SHA256().Hash(GUtilities.StringToByteArray(password)));
            return result;
        }


        public AccessToken Login(string userName, string password)
        {
            if (!this._TransientAuthenticationServicePersistence.UserWithNameExists(userName))
            {
                throw new BadRequestException((int)System.Net.HttpStatusCode.BadRequest, "User does not exist.");
            }
            this._TransientAuthenticationServicePersistence.GetUserByName(userName);
            UserType user = this.GetUserByNameTyped(userName);
            if (this.Hash(password) == user.PasswordHash)
            {
                AccessToken newAccessToken = new AccessToken();
                newAccessToken.Value = Guid.NewGuid().ToString();
                newAccessToken.ExpiredMoment = this._TimeService.GetCurrentTime().AddDays(1);
                user.AccessToken.Add(newAccessToken);
                return newAccessToken;
            }
            else
            {
                throw new BadRequestException((int)System.Net.HttpStatusCode.Unauthorized, "Invalid password.");
            }
        }

        public void Logout(AccessToken accessToken)
        {
            if (this._TransientAuthenticationServicePersistence.AccessTokenExists(accessToken.Value, out User user))
            {
                user.AccessToken = user.AccessToken.Where(at => at.Value != accessToken.Value).ToHashSet();
            }
            else
            {
                throw new BadRequestException(400, "Accesstoken not found");
            }
        }

        public void LogoutEverywhere(string userId)
        {
            UserType user = this._TransientAuthenticationServicePersistence.GetUserById(userId);
            user.RefreshToken.Clear();
            user.AccessToken.Clear();
        }

        public bool UserExists(string userId)
        {
            return this._TransientAuthenticationServicePersistence.UserWithIdExists(userId);
        }

        public bool AccessTokenIsValid(string accessToken)
        {

            if (this._TransientAuthenticationServicePersistence.AccessTokenExists(accessToken, out User user))
            {
                AccessToken at = user.AccessToken.Where(at => at.Value == accessToken).First();
                return this._TimeService.GetCurrentTime() < at.ExpiredMoment;
            }
            else
            {
                return false;
            }
        }

        public ISet<UserType> GetAllUserTyped()
        {
            return this._TransientAuthenticationServicePersistence.GetAllUsers().Values.ToHashSet();
        }

        public UserType GetUserTyped(string userId)
        {
            return this._TransientAuthenticationServicePersistence.GetUserById(userId);
        }

        public string GetUserName(string accessToken)
        {
            if (this._TransientAuthenticationServicePersistence.AccessTokenExists(accessToken, out User user))
            {
                return user.Name;
            }
            else
            {
                throw new BadRequestException(400, "Accesstoken not found");
            }
        }

        public void RemoveUser(string userId)
        {
            this._TransientAuthenticationServicePersistence.SetAllUsers(this._TransientAuthenticationServicePersistence.GetAllUsers().Values.Where(u => u.Id != userId).ToHashSet());
        }

        public ISet<User> GetAllUser()
        {
            return this._TransientAuthenticationServicePersistence.GetAllUsers().Values.Cast<User>().ToHashSet();
        }

        public User GetUser(string userId)
        {
            return this._TransientAuthenticationServicePersistence.GetUserById(userId);
        }

        public Role GetRoleByName(string roleName)
        {
            return this._TransientAuthenticationServicePersistence.GetAllRoles().Where(r => r.Name == roleName).First();
        }
        public Role GetRoleById(string roleId)
        {
            return this._TransientAuthenticationServicePersistence.GetAllRoles().Where(r => r.Id == roleId).First();
        }
        public void EnsureUserHasRole(string userId, string roleId)
        {
            Role role = this.GetRoleById(roleId);
            UserType user = this._TransientAuthenticationServicePersistence.GetAllUsers()[userId];
            user.Roles.Add(role);
        }

        public void EnsureUserDoesNotHaveRole(string userId, string roleId)
        {
            Role role = this.GetRoleById(roleId);
            UserType user = this._TransientAuthenticationServicePersistence.GetAllUsers()[userId];
            user.Roles.Remove(role);
        }

        public bool UserHasRole(string userId, string roleId)
        {
            Role role = this.GetRoleById(roleId);
            UserType user = this._TransientAuthenticationServicePersistence.GetAllUsers()[userId];
            return user.Roles.Contains(role);
        }

        public bool RoleExists(string roleName)
        {
            return this._TransientAuthenticationServicePersistence.GetAllRoles().Where(r => r.Name == roleName).Any();
        }

        public void EnsureRoleExists(string roleName)
        {
            if (!this.RoleExists(roleName))
            {
                Role newRole =new Role();
                newRole.Id = Guid.NewGuid().ToString();
                newRole.Name = roleName;
                newRole.InheritedRoles = new HashSet<Role>();
                this._TransientAuthenticationServicePersistence.AddRole(newRole);
            }
        }

        public void EnsureRoleDoesNotExist(string roleName)
        {
            if (this.RoleExists(roleName))
            {
                this._TransientAuthenticationServicePersistence.DeleteRoleByName(roleName);
            }
        }
            public ISet<string> GetRolesOfUser(string userId)
        {
            return this._TransientAuthenticationServicePersistence.GetUserById(userId).Roles.Select(r=>r.Name).ToHashSet();
        }

        public void AddUserTyped(UserType user)
        {
            if (this._TransientAuthenticationServicePersistence.UserWithNameExists(user.Name))
            {
                throw new BadRequestException((int)System.Net.HttpStatusCode.BadRequest, "User with this name already exists.");
            }
            this._TransientAuthenticationServicePersistence.AddUser(user);
        }

        public void AddUser(User user)
        {
            this.AddUserTyped((UserType)user);
        }


        public UserType GetUserByNameTyped(string username)
        {
            return this._TransientAuthenticationServicePersistence.GetAllUsers().Where(kvp => kvp.Value.Name == username).First().Value;
        }

        public User GetUserByName(string name)
        {
            return this.GetUserByNameTyped(name);
        }

        public bool UserWithNameExists(string username)
        {
           return this._TransientAuthenticationServicePersistence.GetAllUsers().Where(kvp=>kvp.Value.Name == username).Any();
        }

        public User GetUserById(string userId)
        {
           return this._TransientAuthenticationServicePersistence.GetUserById(userId);
        }

        public User GetUserByAccessToken(string accessToken)
        {
         return this._TransientAuthenticationServicePersistence.GetUserByAccessToken(accessToken);   
        }

        public void AddRole(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
