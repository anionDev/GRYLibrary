namespace GRYLibrary.Core.APIServer.Settings.Configuration
{
    public class ServerConfiguration
    {
        public string Domain { get; set; }
        public Protocol Protocol { get; set; }
        public string DevelopmentCertificatePasswordHex { get; set; }
        public string DevelopmentCertificatePFXHex { get; set; }
        public bool HostAPISpecificationForInNonDevelopmentEnvironment { get; set; } = false;
        public const string APIRoutePrefix = "/API";
        public const string APISpecificationDocumentName = "APISpecification";
        public const string ResourcesSubPath = $"Other/Resources";
        public const string APIResourcesSubPath = $"{APIRoutePrefix}/{ResourcesSubPath}";
        public const string TermsOfServiceURLSubPath = $"{APIResourcesSubPath}/Information/TermsOfService";
        public const string ContactURLSubPath = $"{APIResourcesSubPath}/Information/Contact";
        public const string LicenseURLSubPath = $"{APIResourcesSubPath}/Information/License";
        public string GetServerAddress()
        {
            return $"{this.Protocol.GetProtocol()}://{this.Domain}:{this.Protocol.Port}";
        }
    }
}
