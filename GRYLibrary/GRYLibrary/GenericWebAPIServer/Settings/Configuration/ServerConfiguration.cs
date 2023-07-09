using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    public class ServerConfiguration
    {
        public string Domain { get; set; }
        public Protocol Protocol { get; set; }
        public bool HostAPISpecificationForInNonDevelopmentEnvironment { get; set; } = false;
        public const string APIRoutePrefix = "/API";
        public const string APIDocumentationDocumentName = "Specification";
        public const string TermsOfServiceURLSubPath = $"{APIRoutePrefix}/Other/Resources/Information/TermsOfService";
        public const string ContactURLSubPath = $"{APIRoutePrefix}/Other/Resources/Information/Contact";
        public const string LicenseURLSubPath = $"{APIRoutePrefix}/Other/Resources/Information/License";
        public ServerConfiguration() { }
        public string GetServerAddress()
        {
            return $"{this.Protocol.GetProtocol()}://{this.Domain}:{this.Protocol.Port}";
        }
    }
}
