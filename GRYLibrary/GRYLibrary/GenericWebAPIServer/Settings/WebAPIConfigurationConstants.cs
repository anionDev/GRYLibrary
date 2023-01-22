﻿using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IWebAPIConfigurationConstants
    {
        public ConcreteEnvironments.Environment TargetEnvironmentType { get; }
        public string AppName { get; }
        public string AppVersion { get; }
        public string ConfigurationFileName { get; }
    }
    public class WebAPIConfigurationConstants : IWebAPIConfigurationConstants
    {
        public ConcreteEnvironments.Environment TargetEnvironmentType { get; private set; }
        public string AppName { get; private set; }
        public string AppVersion { get; private set; }
        public string ConfigurationFileName { get; private set; } 
        public WebAPIConfigurationConstants(ConcreteEnvironments.Environment targetEnvironmentType, string appName, string appVersion):this(targetEnvironmentType,appName,appVersion, "WebAPIServerSettings.json")
        {
        }
        public WebAPIConfigurationConstants(ConcreteEnvironments.Environment targetEnvironmentType, string appName, string appVersion, string configurationFileName)
        {
            this.TargetEnvironmentType = targetEnvironmentType;
            this.AppName = appName;
            this.AppVersion = appVersion;
            this.ConfigurationFileName = configurationFileName;
        }
    }
}
