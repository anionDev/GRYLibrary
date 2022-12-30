using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationConstants
    {
        private string _TargetEnvironmentType;
        public string TargetEnvironmentType { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string ConfigurationFileName { get; set; } = "APIServerAppSettings.json";
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
