using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Settings.CommonRoutes;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.FilePath;

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

        public void Initialize(string baseFolder)
        {
            this._BaseFolder = baseFolder;
        }
    }
}
