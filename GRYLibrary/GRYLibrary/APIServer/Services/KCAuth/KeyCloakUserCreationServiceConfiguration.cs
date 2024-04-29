namespace GRYLibrary.Core.APIServer.Services.KCAuth
{
    public class KeyCloakUserCreationServiceConfiguration : IKeyCloakUserCreationServiceConfiguration
    {
        public string PasswordHashAlgorithmIdentifier { get; set; }

    }
}
