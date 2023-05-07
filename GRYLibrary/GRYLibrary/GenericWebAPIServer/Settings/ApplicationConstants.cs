using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares;
using GRYLibrary.Core.GenericWebAPIServer.Settings.CommonRoutes;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.FilePath;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents Application-constants which are not editable by a configuration-file.
    /// </summary>
    public interface IApplicationConstants
    {
        public string ApplicationName { get; set; }
        public string ApplicationDescription { get; set; }
        public Version3 ApplicationVersion { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment Environment { get; set; }
        public AbstractFilePath ConfigurationFolder { get; set; }
        public AbstractFilePath CertificateFolder { get; set; }
        public AbstractFilePath ConfigurationFile { get; set; }
        public AbstractFilePath LogFolder { get; set; }
        public string GetConfigurationFolder();
        public string GetCertificateFolder();
        public string GetConfigurationFile();
        public string GetLogFolder();
        public CommonRoutesInformation CommonRoutes { get; set; }
        public void Initialize(string baseFolder);
        public Type ApiKeyValidatorMiddleware { get; set; }
        public Type BlackListMiddleware { get; set; }
        public Type DDOSProtectionMiddleware { get; set; }
        public Type ExceptionManagerMiddleware { get; set; }
        public Type ObfuscationMiddleware { get; set; }
        public Type RequestCounterMiddleware { get; set; }
        public Type RequestLoggingMiddleware { get; set; }
        public Type WebApplicationFirewallMiddleware { get; set; }
        public ISet<Type> KnownTypes { get; set; }
    }
    public interface IApplicationConstants<AppSpecificConstants> :IApplicationConstants
    {
        public AppSpecificConstants ApplicationSpecificConstants { get; set; }
    }
    public class ApplicationConstants<AppSpecificConstants> :IApplicationConstants<AppSpecificConstants>
    {
        public ApplicationConstants(string applicationName, string appDescription, Version3 applicationVersion, ExecutionMode executionMode, GRYEnvironment environment, AppSpecificConstants applicationSpecificConstants)
        {
            this.ApplicationName = applicationName;
            this.ApplicationDescription = appDescription;
            this.ApplicationVersion = applicationVersion;
            this.ExecutionMode = executionMode;
            this.Environment = environment;
            this.ApplicationSpecificConstants = applicationSpecificConstants;
            this.ConfigurationFolder = AbstractFilePath.FromString("./Configuration");
            this.CertificateFolder = AbstractFilePath.FromString("./Certificates");
            this.ConfigurationFile = AbstractFilePath.FromString("./Configuration.xml");
            this.LogFolder = AbstractFilePath.FromString("./Logs");
        }
        private string _BaseFolder;
        public string ApplicationName { get; set; }
        public string ApplicationDescription { get; set; }
        public Version3 ApplicationVersion { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment Environment { get; set; }
        public AppSpecificConstants ApplicationSpecificConstants { get; set; }
        public AbstractFilePath ConfigurationFolder { get; set; }
        public AbstractFilePath CertificateFolder { get; set; }
        public AbstractFilePath ConfigurationFile { get; set; }
        public AbstractFilePath LogFolder { get; set; }
        public string GetConfigurationFolder() { return this.ConfigurationFolder.GetPath(this._BaseFolder); }
        public string GetCertificateFolder() { return this.CertificateFolder.GetPath(this.GetConfigurationFolder()); }
        public string GetConfigurationFile() { return this.ConfigurationFile.GetPath(this.GetConfigurationFolder()); }
        public string GetLogFolder() { return this.LogFolder.GetPath(this._BaseFolder); }
        public CommonRoutesInformation CommonRoutes { get; set; } = new HostCommonRoutes();
        public Type ApiKeyValidatorMiddleware { get; set; } = typeof(APIKeyValidator);
        public Type BlackListMiddleware { get; set; } = typeof(BlackList);
        public Type DDOSProtectionMiddleware { get; set; } = typeof(DDOSProtection);
        public Type ExceptionManagerMiddleware { get; set; } = typeof(ExceptionManager);
        public Type ObfuscationMiddleware { get; set; } = typeof(Obfuscation);
        public Type RequestCounterMiddleware { get; set; } = typeof(RequestCounter);
        public Type RequestLoggingMiddleware { get; set; } = typeof(RequestLoggingMiddleware);
        public Type WebApplicationFirewallMiddleware { get; set; } = typeof(WebApplicationFirewall);
        public ISet<Type> KnownTypes { get; set; } = new HashSet<Type>();

        public void Initialize(string baseFolder)
        {
            this._BaseFolder = baseFolder;
        }
    }
}
