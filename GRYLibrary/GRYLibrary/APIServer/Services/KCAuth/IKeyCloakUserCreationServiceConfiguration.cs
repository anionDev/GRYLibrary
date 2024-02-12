namespace GRYLibrary.Core.APIServer.Services.KCAuth
{
    public interface IKeyCloakUserCreationServiceConfiguration
    {
        public string PasswordHashAlgorithmIdentifier {  get; set; }
    }
}
