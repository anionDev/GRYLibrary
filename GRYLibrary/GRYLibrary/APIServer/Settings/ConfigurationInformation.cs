using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GRYLibrary.Core.APIServer.Settings
{
    /// <summary>
    /// Represents a container for technical information which are required to start a WebAPI-server.
    /// </summary>
    public class ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public ConfigurationInformation()
        {
        }
        public APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> APIServerInitializer { get; set; }
        public IServiceCollection ServiceCollection { get; set; }
        public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> PersistedAPIServerConfiguration { get; set; }
        public CommandlineParameterType CommandlineParameter { get; set; }
    }
}
