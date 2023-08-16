namespace GRYLibrary.Core.APIServer.Services.KeyCloak
{
    public class KeyCloakServiceSettings :IKeyCloakServiceSettings
    {
        public string URL { get; set; }
        public string Realm { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
