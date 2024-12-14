namespace GRYLibrary.Core.APIServer.Services.KC
{
    public class KeyCloakServiceSettings : IKeyCloakServiceSettings
    {
        public string URL { get; set; }
        public string Realm { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string ClientUsername { get; set; }
        public string ClientPassword { get; set; }
    }
}
