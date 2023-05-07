using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration;
using GRYLibrary.Core.GenericWebAPIServer.Utilities;
using System;

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
        public BlacklistProvider BlackListProvider { get; set; }
        public DDOSProtectionSettings DDOSProtectionSettings { get; set; }
        public ObfuscationSettings ObfuscationSettings { get; set; }
        public ExceptionManagerSettings ExceptionManagerSettings { get; set; }
        public RequestCounterSettings RequestCounterSettings { get; set; }
        public RequestLoggingSettings RequestLoggingSettings { get; set; }
        public WebApplicationFirewallSettings WebApplicationFirewallSettings { get; set; }
        public CredentialsValidatorSettings CredentialsValidatorSettings { get; set; }
        public const string APIRoutePrefix = "API";
        public static string GetAPIDocumentationRoutePrefix() { return $"{APIRoutePrefix}/APIDocumentation"; }
        public ServerConfiguration() { }
        public static ServerConfiguration Create(string domain, GRYEnvironment environment, TLSCertificateInformation tlsCertificateInformation)
        {
            ServerConfiguration result = new ServerConfiguration();
            result.Protocol = HTTPS.Create(tlsCertificateInformation);
            SetCommonSettings(result, environment, domain);
            return result;
        }

        public static ServerConfiguration Create(string domain, GRYEnvironment environment)
        {
            ServerConfiguration result = new ServerConfiguration();
            result.Protocol = new HTTP();
            SetCommonSettings(result, environment, domain);
            return result;
        }
        public const string LocalDomain = "localhost";
        private static void SetCommonSettings(ServerConfiguration result, GRYEnvironment environment, string domain)
        {
            result.Domain = environment is Development ? LocalDomain : domain;
            result.BlackListProvider = new BlacklistProvider();
            result.DDOSProtectionSettings = new DDOSProtectionSettings();
            result.ObfuscationSettings = new ObfuscationSettings();
            result.ExceptionManagerSettings = new ExceptionManagerSettings();
            result.RequestCounterSettings = new RequestCounterSettings();
            result.RequestLoggingSettings = new RequestLoggingSettings() { RequestsLogConfiguration = ServerUtilities.GetLogConfiguration("Requests.log", environment) };
            result.WebApplicationFirewallSettings = new WebApplicationFirewallSettings();
            result.CredentialsValidatorSettings = new CredentialsValidatorSettings();
        }

        public string GetServerAddress()
        {
            return $"{this.Protocol.GetProtocol()}://{this.Domain}:{this.Protocol.Port}";
        }
    }
}
