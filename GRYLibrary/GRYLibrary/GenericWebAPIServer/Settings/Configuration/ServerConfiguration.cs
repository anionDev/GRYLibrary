using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    public class ServerConfiguration
    {
        public string Domain { get; set; }
        public Protocol Protocol { get; set; }
        public bool HostAPISpecificationForInNonDevelopmentEnvironment { get; set; } = false;
        public const string APIRoutePrefix = "/API";
        public const string APIDocumentationDocumentName  = "APIDocumentation";
        public const string APIDocumentationRoutePrefix =$"{APIRoutePrefix}/{APIDocumentationDocumentName}"; 
        public string TermsOfServiceURLSubPath { get; set; } = $"{APIDocumentationRoutePrefix}/Information/TermsOfService";
        public string ContactURLSubPath { get; set; } = $"{APIDocumentationRoutePrefix}/Information/Contact";
        public string LicenseURLSubPath { get; set; } = $"{APIDocumentationRoutePrefix}/Information/License";
        public ServerConfiguration() { }
        public static ServerConfiguration Create(string domain, GRYEnvironment environment, TLSCertificateInformation tlsCertificateInformation)
        {
            ServerConfiguration result = new ServerConfiguration
            {
                Protocol = tlsCertificateInformation == null ? HTTP.Create() : HTTPS.Create(tlsCertificateInformation)
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
