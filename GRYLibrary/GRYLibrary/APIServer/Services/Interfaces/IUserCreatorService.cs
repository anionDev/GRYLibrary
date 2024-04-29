using GRYLibrary.Core.APIServer.CommonDBTypes;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IUserCreatorService
    {
        /// <returns>Returns a newly created and not persisted <see cref="User"/>-object.</returns>
        public User CreateUser(string name, string passwordHash);
        public string Hash(string password);
    }
}
