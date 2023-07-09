namespace GRYLibrary.Core.GenericWebAPIServer.Services.KeyCloak
{
    public interface IKeyCloakServiceSettings
    {
        public string URL { get; set; }
        public string Realm { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
