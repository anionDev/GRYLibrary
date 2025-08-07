namespace GRYLibrary.Core.APIServer.Services.OpenIDConnectAuth
{
    public interface IOpenIDConnectUserCreationServiceConfiguration
    {
        public string PasswordHashAlgorithmIdentifier { get; set; }
    }
}
