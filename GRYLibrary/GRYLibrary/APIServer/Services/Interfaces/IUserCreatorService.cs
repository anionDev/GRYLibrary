using GRYLibrary.Core.APIServer.CommonDBTypes;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IUserCreatorService: IUserCreatorService<User>
    {
    }
    public interface IUserCreatorService<UserType> where UserType : User
    {
        /// <returns>Returns a newly created and not persisted <typeparamref name="UserType"/>-object.</returns>
        UserType CreateUser(string name, string passwordHash);
    }
}
