using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.GeneralPurposeLogger;
using Microsoft.Extensions.DependencyInjection;

namespace GRYLibrary.Core.APIServer.Settings
{
    public class ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
    {

        public ConfigurationInformation(IServiceCollection serviceCollection, IApplicationConstants<ApplicationSpecificConstants> applicationConstants,
            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration, CommandlineParameterType commandlineParameter,
            ExecutionMode executionMode, GRYEnvironment environment, IGeneralLogger logger)
        {
            this.ServiceCollection = serviceCollection;
            this.ApplicationConstants = applicationConstants;
            this.PersistedAPIServerConfiguration = persistedApplicationSpecificConfiguration;
            this.CommandlineParameter = commandlineParameter;
            this.ExecutionMode = executionMode;
            this.Environment = environment;
            this.Logger = logger;
        }

        public IServiceCollection ServiceCollection { get; set; }
        public IApplicationConstants<ApplicationSpecificConstants> ApplicationConstants { get; set; }
        public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> PersistedAPIServerConfiguration { get; set; }
        public CommandlineParameterType CommandlineParameter { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment Environment { get; set; }
        public IGeneralLogger Logger { get; set; }
    }
}
