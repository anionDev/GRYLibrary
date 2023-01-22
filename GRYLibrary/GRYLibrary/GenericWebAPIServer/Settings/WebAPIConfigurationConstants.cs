using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IWebAPIConfigurationConstants
    {
        public string TargetEnvironmentType { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string ConfigurationFileName { get; set; } 
        public Environment GetTargetEnvironmentType();
    }
    public class WebAPIConfigurationConstants: IWebAPIConfigurationConstants
    {
        public string TargetEnvironmentType { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string ConfigurationFileName { get; set; } = "APIServerSettings.json";
        public Environment GetTargetEnvironmentType()
        {
            if (TargetEnvironmentType == null)
            {
                throw new ArgumentException($"Value for {nameof(TargetEnvironmentType)} is null.");
            }
            if (TargetEnvironmentType == nameof(Development))
            {
                return Development.Instance;
            }
            else if (TargetEnvironmentType == nameof(QualityCheck))
            {
                return QualityCheck.Instance;
            }
            else if (TargetEnvironmentType == nameof(Productive))
            {
                return Productive.Instance;
            }
            else
            {
                throw new ArgumentException($"Unknown value for {nameof(TargetEnvironmentType)}: {TargetEnvironmentType}");
            }
        }
    }
}
