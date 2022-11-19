namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebServerSettings
    {
        public ulong MaxRequestBodySize { get; set; }
        public bool UseHTTPS { get; set; }
        public EncryptionSettings EncryptionSettings { get; set; }

    }
}
