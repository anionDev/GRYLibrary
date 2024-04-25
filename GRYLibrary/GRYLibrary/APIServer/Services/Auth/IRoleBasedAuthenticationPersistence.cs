using GRYLibrary.Core.APIServer.CommonDBTypes;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    public interface IRoleBasedAuthenticationPersistence
    {
        void CreateUser(string username, string password);
        ISet<User> GetAllUsers();
        User GetUser(string id);
        User GetUserByName(string username);
        void Update(User user);
        void DeleteUser(string id);
        bool UserWithSpecificNameExists(string username);
    }
}
