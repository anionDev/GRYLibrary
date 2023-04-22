using GRYLibrary.Core.GenericWebAPIServer.Services;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents configuration-settings for a WebAPI-server which are allowed to be changed in a configuration-file.
    /// </summary>
    public class WebServerConfiguration
    {
        public string SwaggerDocumentName { get; set; } = "APISpecification";
        public ushort Port { get; set; } = 80;
        public string APIRoutePrefix { get; set; } = "API";
        public string TLSCertificatePasswordFile { get; set; } = null;
        public string TLSCertificatePFXFilePath { get; set; } = null;
        public BlacklistProvider BlackListProvider { get; set; } //TODO use interface-type for middleware-propertys
        public DDOSProtectionSettings DDOSProtectionSettings { get; set; }
        public ObfuscationSettings ObfuscationSettings { get; set; }
        public ExceptionManagerSettings ExceptionManagerSettings { get; set; }
        public RequestCounterSettings RequestCounterSettings { get; set; }
        public RequestLoggingSettings RequestLoggingSettings { get; set; }
        public WebApplicationFirewallSettings WebApplicationFirewallSettings { get; set; }
        public APIKeyValidatorSettings APIKeyValidatorSettings { get; set; }
    }
}