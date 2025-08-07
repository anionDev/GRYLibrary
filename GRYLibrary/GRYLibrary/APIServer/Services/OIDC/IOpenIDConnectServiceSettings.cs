namespace GRYLibrary.Core.APIServer.Services.OpenIDConnect
{
    public interface IOpenIDConnectServiceSettings
    {
        public string URL { get; set; }
        public string Label { get; set; }
        public string ClientUsername { get; set; }
        public string ClientPassword { get; set; }
    }
}
