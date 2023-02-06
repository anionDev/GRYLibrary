using GRYLibrary.Core.GenericWebAPIServer.Services;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebServerSettings
    {
        public string SwaggerDocumentName { get; set; } = "APISpecification";
        public ushort Port { get; set; } = 80;
        public string APIRoutePrefix { get; set; } = "API";
        public string TLSCertificatePasswordFile { get; set; } = null;
        public string TLSCertificatePFXFilePath { get; set; } = null;
        public IBlacklistProvider BlackListProvider { get; set; } = new BlacklistProvider();
        public IDDOSProtectionSettings DDOSProtectionSettings { get; set; } = new DDOSProtectionSettings();
        public IObfuscationSettings ObfuscationSettings { get; set; } = new ObfuscationSettings();
        public IExceptionManagerSettings ExceptionManagerSettings { get; set; } = new ExceptionManagerSettings();
        public IRequestCounterSettings RequestCounterSettings { get; set; } = new RequestCounterSettings();
        public IRequestLoggingSettings RequestLoggingSettings { get; set; } = new RequestLoggingSettings();
        public IWebApplicationFirewallSettings WebApplicationFirewallSettings { get; set; } = new WebApplicationFirewallSettings();
    }
}
