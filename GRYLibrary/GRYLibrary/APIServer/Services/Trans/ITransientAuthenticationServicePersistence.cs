using GRYLibrary.Core.APIServer.CommonDBTypes;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    /// Represetns a authenticationservice-persistence where userdata (user, roles, accesstoken, etc.) will be stored transient so that they will be removed after every restart.
    public interface ITransientAuthenticationServicePersistence<UserType>
        where UserType : User
    {
        public IDictionary<string,UserType> GetAllUsers();
        public void SetAllUsers(ISet<UserType> users);
        public IDictionary<string,Role> GetAlRoles();
        public void SetAllRoles(ISet<Role> roles);
        bool AccessTokenExists(string accessToken, out User user);
        void AddUser(UserType newUser);
        bool UserWithNameExists(string userName);
        bool UserWithIdExists(string userId);
        UserType GetUserById(object userId);
        UserType GetUserByName(object userName);
        void RemoveUser(UserType user);
    }
}
