using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
namespace GRYLibrary.Core.APIServer.Settings
{
    /// <summary>
    /// Represents a container for technical information which are required to start a WebAPI-server.
    /// </summary>
    public class FunctionalInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public FunctionalInformation(
            InitializationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> initializationInformation,
            IServiceCollection serviceCollection,
            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedAPIServerConfiguration)
        {
            this.InitializationInformation = initializationInformation;
            this.ServiceCollection = serviceCollection;
            this.PersistedAPIServerConfiguration = persistedAPIServerConfiguration;
        }
        public InitializationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> InitializationInformation { get; internal set; }
        public IServiceCollection ServiceCollection { get; internal set; }
        public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> PersistedAPIServerConfiguration { get; internal set; }
        public Action PreRun { get; set; } = () => { };
        public Action PostRun { get; set; } = () => { };
        public ISet<FilterDescriptor> Filter { get; set; } = new HashSet<FilterDescriptor>();
    }
}
