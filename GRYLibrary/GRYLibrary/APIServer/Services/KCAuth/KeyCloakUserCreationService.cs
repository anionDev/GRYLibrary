using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Crypto;
using System;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Services.KCAuth
{
    public class KeyCloakUserCreationService : IUserCreatorService
    {
        private readonly IKeyCloakUserCreationServiceConfiguration _IKeyCloakUserCreationServiceConfiguration;
        public KeyCloakUserCreationService(IKeyCloakUserCreationServiceConfiguration keyCloakUserCreationServiceConfiguration)
        {
            this._IKeyCloakUserCreationServiceConfiguration = keyCloakUserCreationServiceConfiguration;
        }
        public virtual User CreateUser(string name, string password)
        {
            User user = new User();
            user.Id = Guid.NewGuid().ToString();
            user.Name = name;
            user.PasswordHash = Tools.GetHashAlgorithm(this._IKeyCloakUserCreationServiceConfiguration.PasswordHashAlgorithmIdentifier).Hash(password);
            return user;
        }

        public string Hash(string password)
        {
            return GUtilities.ByteArrayToHexString(new Argon2().Hash(GUtilities.StringToByteArray(password)));
        }
    }
}
