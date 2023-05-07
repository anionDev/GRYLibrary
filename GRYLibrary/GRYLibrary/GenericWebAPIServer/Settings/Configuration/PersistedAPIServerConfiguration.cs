using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Utilities;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;
using System;

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
        public static PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> Create<PersistedAppSpecificConfiguration>(string domain, PersistedAppSpecificConfiguration persistedApplicationSpecificConfiguration, GRYEnvironment environment, string fallbackCertificatePasswordFileContentHex, string fallbackCertificatePFXFileContentHex)
            where PersistedAppSpecificConfiguration : new()
        {
            PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> result = new PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>();
            result.ServerConfiguration = ServerConfiguration.Create(domain, environment, TLSCertificateInformation.Create(AbstractFilePath.FromString($"./Certificate.{domain}.password"), AbstractFilePath.FromString($"./Certificate.{domain}.pfx"), fallbackCertificatePasswordFileContentHex, fallbackCertificatePFXFileContentHex));
            result.ApplicationLogConfiguration = ServerUtilities.GetLogConfiguration("Server.log", environment);
            result.ApplicationSpecificConfiguration = persistedApplicationSpecificConfiguration;
            return result;
        }

    }
}
