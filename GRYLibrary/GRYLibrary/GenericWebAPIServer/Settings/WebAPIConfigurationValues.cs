using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Services;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationValues
    {
        internal IGeneralLogger Logger;
        public IBlacklistProvider BlackListProvider { get; set; } = new BlacklistProvider();
        public IDDOSProtectionSettings DDOSProtectionSettings { get; set; } = new DDOSProtectionSettings();
        public IObfuscationSettings ObfuscationSettings { get; set; } = new ObfuscationSettings();
        public IExceptionManagerSettings ExceptionManagerSettings { get; set; } = new ExceptionManagerSettings();
        public IRequestCounterSettings RequestCounterSettings { get; set; } = new RequestCounterSettings();
        public IWebApplicationFirewallSettings WebApplicationFirewallSettings { get; set; } = new WebApplicationFirewallSettings();
        public WebAPIConfigurationConstants WebAPIConfigurationConstants { get; set; }
        public WebAPIConfigurationVariables WebAPIConfigurationVariables { get; set; }
        public ExecutionMode ExecutionMode { get; set; } = GenericWebAPIServer.GetExecutionMode();
        public bool RethrowInitializationExceptions { get; set; } = false;
    }
}
