namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebServerSettings
    {
        public string SwaggerDocumentName { get; set; } = "APISpecification";
        public ushort Port { get; set; } = 80;
        public string APIRoutePrefix { get; set; } = "API";
        public string TLSCertificatePasswordFile { get; set; } = null;
        public string TLSCertificatePFXFilePath { get; set; } = null;
    }
}
