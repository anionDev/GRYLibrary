using System;

namespace GRYLibrary.Core.APIServer.Settings
{
    public class APIServerConfiguration<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
    {
        internal InitializationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> InitializationInformation { get; set; }
        public Action<InitializationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>> SetInitialzationInformationAction { get; set; }
        internal FunctionalInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> FunctionalInformation { get; set; }
        public Action<FunctionalInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>> SetFunctionalInformationAction { get; set; }
    }
}
