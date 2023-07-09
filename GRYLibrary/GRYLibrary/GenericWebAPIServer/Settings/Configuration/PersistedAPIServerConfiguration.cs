using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    /// <summary>
    /// Represents Application-constants which are editable by a configuration-file.
    /// </summary>
    public interface IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public PersistedApplicationSpecificConfiguration ApplicationSpecificConfiguration { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
        public GRYLogConfiguration ApplicationLogConfiguration { get; set; }
    }

    public class PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> :IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public ServerConfiguration ServerConfiguration { get; set; }
        public GRYLogConfiguration ApplicationLogConfiguration { get; set; }
        public PersistedApplicationSpecificConfiguration ApplicationSpecificConfiguration { get; set; }
        public static PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> Create<PersistedAppSpecificConfiguration>(string domain, PersistedAppSpecificConfiguration persistedApplicationSpecificConfiguration, GRYEnvironment environment, string fallbackCertificatePasswordFileContentHex, string fallbackCertificatePFXFileContentHex, string codeunitName)
            where PersistedAppSpecificConfiguration : new()
        {
            domain = environment is Development ? $"{codeunitName.ToLower()}.test.local" : domain;
            ServerConfiguration serverConfiguration = new ServerConfiguration();
            TLSCertificateInformation tlsCertificateInformation = fallbackCertificatePFXFileContentHex == null ? null : new TLSCertificateInformation
            {
                CertificatePFXFile = AbstractFilePath.FromString($"./{domain}.pfx"),
                CertificatePasswordFile = AbstractFilePath.FromString($"./{domain}.password"),
                FallbackCertificatePasswordFileContentHex = fallbackCertificatePasswordFileContentHex,
                FallbackCertificatePFXFileContentHex = fallbackCertificatePFXFileContentHex
            };
            serverConfiguration.Protocol = tlsCertificateInformation == null ? HTTP.Create() : HTTPS.Create(tlsCertificateInformation);
            serverConfiguration.Domain = domain;
            PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> result = new PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>
            {
                ServerConfiguration = serverConfiguration,
                ApplicationLogConfiguration = GRYLogConfiguration.GetCommonConfiguration("Server.log", environment is Development),
                ApplicationSpecificConfiguration = persistedApplicationSpecificConfiguration
            };
            return result;
        }
    }
}
