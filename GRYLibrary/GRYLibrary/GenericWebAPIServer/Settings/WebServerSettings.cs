using GRYLibrary.Core.GenericWebAPIServer.Services;
using System.Xml.Serialization;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebServerSettings
    {
        public string SwaggerDocumentName { get; set; } = "APISpecification";
        public ushort Port { get; set; } = 80;
        public string APIRoutePrefix { get; set; } = "API";
        public string TLSCertificatePasswordFile { get; set; } = null;
        public string TLSCertificatePFXFilePath { get; set; } = null;
        public BlacklistProvider BlackListProvider { get; set; } = new BlacklistProvider();
        public DDOSProtectionSettings DDOSProtectionSettings { get; set; } = new DDOSProtectionSettings();
        public ObfuscationSettings ObfuscationSettings { get; set; } = new ObfuscationSettings();
        public ExceptionManagerSettings ExceptionManagerSettings { get; set; } = new ExceptionManagerSettings();
        public RequestCounterSettings RequestCounterSettings { get; set; } = new RequestCounterSettings();
        public RequestLoggingSettings RequestLoggingSettings { get; set; } = new RequestLoggingSettings();
        public WebApplicationFirewallSettings WebApplicationFirewallSettings { get; set; } = new WebApplicationFirewallSettings();
    }
}
