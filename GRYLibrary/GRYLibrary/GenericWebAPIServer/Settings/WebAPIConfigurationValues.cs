using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationValues<ConfigurationConstantsType, ConfigurationVariablesType>
        where ConfigurationConstantsType : IWebAPIConfigurationConstants
        where ConfigurationVariablesType : IWebAPIConfigurationVariables
    {
        internal IGeneralLogger Logger;
        public IBlacklistProvider BlackListProvider { get; set; } = new BlacklistProvider();
        public IDDOSProtectionSettings DDOSProtectionSettings { get; set; } = new DDOSProtectionSettings();
        public IObfuscationSettings ObfuscationSettings { get; set; } = new ObfuscationSettings();
        public IExceptionManagerSettings ExceptionManagerSettings { get; set; } = new ExceptionManagerSettings();
        public IRequestCounterSettings RequestCounterSettings { get; set; } = new RequestCounterSettings();
        public IWebApplicationFirewallSettings WebApplicationFirewallSettings { get; set; } = new WebApplicationFirewallSettings();
        public ConfigurationConstantsType WebAPIConfigurationConstants { get; set; }
        public ConfigurationVariablesType WebAPIConfigurationVariables { get; set; }
        public ExecutionMode ExecutionMode { get; set; } = GenericWebAPIServer.GetExecutionMode();
        public bool RethrowInitializationExceptions { get; set; } = false;
        public string[] CommandlineArguments { get; set; } = Array.Empty<string>();
    }
}
