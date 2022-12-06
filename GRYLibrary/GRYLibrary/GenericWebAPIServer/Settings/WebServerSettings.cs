namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebServerSettings
    {
        public ulong MaxRequestBodySize { get; set; } = ulong.MaxValue;
        public bool UseHTTPS { get; set; } = false;
        public EncryptionSettings EncryptionSettings { get; set; } = null;
        public string BasePath { get; set; } = "/API";
        public const string APIExplorerSubRouter = "/APIExplorer";
    }
}
