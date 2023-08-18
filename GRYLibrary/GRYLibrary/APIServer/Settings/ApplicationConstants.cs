using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.FilePath;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Settings
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
        public AbstractFilePath DataFolder { get; set; }
        public AbstractFilePath ConfigurationFolder { get; set; }
        public AbstractFilePath CertificateFolder { get; set; }
        public AbstractFilePath ConfigurationFile { get; set; }
        public AbstractFilePath LogFolder { get; set; }
        public bool ThrowErrorIfConfigurationDoesNotExistInProduction { get; set; }
        public string GetDataFolder();
        public string GetConfigurationFolder();
        public string GetCertificateFolder();
        public string GetConfigurationFile();
        public string GetLogFolder();
        public CommonRoutesHostInformation CommonRoutes { get; set; }
        public void Initialize(string baseFolder);
        public Type AuthenticationMiddleware { get; set; }
        public Type AuthorizationMiddleware { get; set; }
        public Type BlackListMiddleware { get; set; }
        public Type CaptchaMiddleware { get; set; }
        public Type DDOSProtectionMiddleware { get; set; }
        public Type ExceptionManagerMiddleware { get; set; }
        public Type ObfuscationMiddleware { get; set; }
        public Type RequestCounterMiddleware { get; set; }
        public Type RequestLoggingMiddleware { get; set; }
        public Type WebApplicationFirewallMiddleware { get; set; }
        public IList<Type> CustomMiddlewares { get; set; }
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
            this.DataFolder = AbstractFilePath.FromString("./Data");
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
        public AbstractFilePath DataFolder { get; set; }
        public AbstractFilePath ConfigurationFolder { get; set; }
        public AbstractFilePath CertificateFolder { get; set; }
        public AbstractFilePath ConfigurationFile { get; set; }
        public AbstractFilePath LogFolder { get; set; }
        public bool ThrowErrorIfConfigurationDoesNotExistInProduction { get; set; } = false;
        public string GetDataFolder() { return this.DataFolder.GetPath(this._BaseFolder); }
        public string GetConfigurationFolder() { return this.ConfigurationFolder.GetPath(this._BaseFolder); }
        public string GetCertificateFolder() { return this.CertificateFolder.GetPath(this.GetConfigurationFolder()); }
        public string GetConfigurationFile() { return this.ConfigurationFile.GetPath(this.GetConfigurationFolder()); }
        public string GetLogFolder() { return this.LogFolder.GetPath(this._BaseFolder); }
        public CommonRoutesHostInformation CommonRoutes { get; set; } = new HostCommonRoutes();
        public Type AuthenticationMiddleware { get; set; } = null;
        public Type AuthorizationMiddleware { get; set; } = null;
        public Type BlackListMiddleware { get; set; } = null;
        public Type CaptchaMiddleware { get; set; } = null;
        public Type DDOSProtectionMiddleware { get; set; } = null;
        public Type ExceptionManagerMiddleware { get; set; } = null;
        public Type ObfuscationMiddleware { get; set; } = null;
        public Type RequestCounterMiddleware { get; set; } = null;
        public Type RequestLoggingMiddleware { get; set; } = null;
        public Type WebApplicationFirewallMiddleware { get; set; } = null;
        public IList<Type> CustomMiddlewares { get; set; } = new List<Type>();
        public ISet<Type> KnownTypes { get; set; } = new HashSet<Type>();

        public void Initialize(string baseFolder)
        {
            this._BaseFolder = baseFolder;
        }
    }
}
