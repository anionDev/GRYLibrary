using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerGen;
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
            WebApplicationBuilder webApplicationBuilder,
            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedAPIServerConfiguration,
            Logging.GeneralPurposeLogger.IGeneralLogger logger)
        {
            this.InitializationInformation = initializationInformation;
            this.WebApplicationBuilder = webApplicationBuilder;
            this.PersistedAPIServerConfiguration = persistedAPIServerConfiguration;
            this.Logger = logger;
        }
        public InitializationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> InitializationInformation { get; internal set; }
        public WebApplicationBuilder WebApplicationBuilder { get; internal set; }
        public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> PersistedAPIServerConfiguration { get; internal set; }
        public ISet<FilterDescriptor> Filter { get; set; } = new HashSet<FilterDescriptor>();
        public Logging.GeneralPurposeLogger.IGeneralLogger Logger { get; set; }
    }
}
