namespace GRYLibrary.Core.APIServer.Services.KC
{
    public interface IKeyCloakServiceSettings
    {
        public string URL { get; set; }
        public string Realm { get; set; }
        public string AdminClientUsername { get; set; }
        public string AdminClientPassword { get; set; }
    }
}
