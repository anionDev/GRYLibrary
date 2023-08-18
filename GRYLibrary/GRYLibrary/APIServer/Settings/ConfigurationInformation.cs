using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.GeneralPurposeLogger;
using Microsoft.Extensions.DependencyInjection;

namespace GRYLibrary.Core.APIServer.Settings
{
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
        public IGeneralLogger Logger { get; set; }
    }
}
