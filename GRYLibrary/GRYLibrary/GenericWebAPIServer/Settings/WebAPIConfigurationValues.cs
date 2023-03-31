using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Services;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationValues<ConfigurationConstantsType, ConfigurationVariablesType>
        where ConfigurationConstantsType : IWebAPIConfigurationConstants
        where ConfigurationVariablesType : IWebAPIConfigurationVariables
    {
        internal IGeneralLogger Logger;
        public ConfigurationConstantsType WebAPIConfigurationConstants { get; set; }
        public ConfigurationVariablesType WebAPIConfigurationVariables { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public bool RethrowInitializationExceptions { get; set; }
        public string[] CommandlineArguments { get; set; }
    }
}