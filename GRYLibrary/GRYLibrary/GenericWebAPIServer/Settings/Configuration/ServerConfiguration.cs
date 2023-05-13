using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    public class ServerConfiguration
    {
        public string Domain { get; set; }
        public Protocol Protocol { get; set; }
        public string TermsOfServiceURLSubPath { get; set; } = "/API/APIDocumentation/Information/TermsOfService";
        public string ContactURLSubPath { get; set; } = "/API/APIDocumentation/Information/Contact";
        public string LicenseURLSubPath { get; set; } = "/API/APIDocumentation/Information/License";
        public bool HostAPISpecificationForInNonDevelopmentEnvironment { get; set; } = false;
        public string APIDocumentationDocumentName { get; set; } = "APISpecification";
        public const string APIRoutePrefix = "API";
        public static string GetAPIDocumentationRoutePrefix() { return $"{APIRoutePrefix}/APIDocumentation"; }
        public ServerConfiguration() { }
        public static ServerConfiguration Create(string domain, GRYEnvironment environment, TLSCertificateInformation tlsCertificateInformation)
        {
            ServerConfiguration result = new ServerConfiguration
            {
                Protocol = HTTPS.Create(tlsCertificateInformation)
            };
            SetCommonSettings(result, environment, domain);
            return result;
        }

        public static ServerConfiguration Create(string domain, GRYEnvironment environment)
        {
            ServerConfiguration result = new ServerConfiguration
            {
                Protocol = new HTTP()
            };
            SetCommonSettings(result, environment, domain);
            return result;
        }
        public const string LocalDomain = "localhost";
        private static void SetCommonSettings(ServerConfiguration result, GRYEnvironment environment, string domain)
        {
            result.Domain = environment is Development ? LocalDomain : domain;
        }

        public string GetServerAddress()
        {
            return $"{this.Protocol.GetProtocol()}://{this.Domain}:{this.Protocol.Port}";
        }
    }
}
