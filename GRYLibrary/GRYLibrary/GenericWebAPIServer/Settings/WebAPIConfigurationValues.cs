using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.Log;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationValues<ConfigurationConstantsType, ConfigurationVariablesType>
        where ConfigurationConstantsType : IWebAPIConfigurationConstants
        where ConfigurationVariablesType : IWebAPIConfigurationVariables
    {
        internal IGeneralLogger Logger;
        public ConfigurationConstantsType WebAPIConfigurationConstants { get; set; }
        public ConfigurationVariablesType WebAPIConfigurationVariables { get; set; }
        public ExecutionMode ExecutionMode { get; set; } = GenericWebAPIServer.GetExecutionMode();
        public bool RethrowInitializationExceptions { get; set; } = false;
        public string[] CommandlineArguments { get; set; } = Array.Empty<string>();
    }
}
