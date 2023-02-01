using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IWebAPIConfigurationConstants
    {
        public GRYEnvironment TargetEnvironmentType { get; }
        public string AppName { get; }
        public string AppVersion { get; }
        public string ConfigurationFile { get; }
    }
    public class WebAPIConfigurationConstants : IWebAPIConfigurationConstants
    {
        public GRYEnvironment TargetEnvironmentType { get; private set; }
        public string AppName { get; private set; }
        public string AppVersion { get; private set; }
        public string ConfigurationFile { get; private set; }
        public WebAPIConfigurationConstants(ConcreteEnvironments.GRYEnvironment targetEnvironmentType, string appName, string appVersion, string configurationFile)
        {
            this.TargetEnvironmentType = targetEnvironmentType;
            this.AppName = appName;
            this.AppVersion = appVersion;
            this.ConfigurationFile = configurationFile;
        }
    }
}
