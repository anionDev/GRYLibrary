namespace GRYLibrary.Core.APIServer.Services.KeyCloak
{
    public interface IKeyCloakService: IAuthenticationService { 
        public IKeyCloakServiceSettings Settings { get; }
    }
}
