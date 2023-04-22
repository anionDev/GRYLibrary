using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes.Visitors;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using System.IO;
using System.Reflection;
using GRYLibrary.Core.Miscellaneous;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class GeneralWebAPIConfigInitializationInformation
    {
        public string AppName { get; set; }
        public Version3 AppVersion { get; set; }
        public string AppDescription { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment TargetEnvironmentType { get; set; }
        public string Domain { get; set; }
        public bool UseHTTPS { get; set; }
        public string NonProductiveCertificatePasswordHex { get; set; }
        public string NonProductiveCertificatePFXHex { get; set; }
        public string ProgramFolder { get; set; }
        public string BaseFolder { get; set; }
        public string ConfigurationFolder { get; set; }
        public string LogFolder { get; set; }
        public string LogFile { get; set; }
        public string CertificateFolder { get; set; }
        public string TLSCertificatePFXFilePath { get; set; }
        public string TLSCertificatePasswordFile { get; set; }
        public Action PreRun { get;  set; }
        public Action PostRun { get; set; }
        public Action<IServiceCollection> initializeServices{ get; set; }

        public GeneralWebAPIConfigInitializationInformation(string appName, Version3 appVersion, string appDescription, ExecutionMode executionMode, GRYEnvironment targetEnvironmentType, string domain, bool useHTTPS, string nonProductiveCertificatePasswordHex, string nonProductiveCertificatePFXHex)
        {
            this.AppName = appName;
            this.AppVersion = appVersion;
            this.AppDescription = appDescription;
            this.ExecutionMode = executionMode;
            this.TargetEnvironmentType = targetEnvironmentType;
            this.Domain = domain;
            this.UseHTTPS = useHTTPS;
            this.NonProductiveCertificatePasswordHex = nonProductiveCertificatePasswordHex;
            this.NonProductiveCertificatePFXHex = nonProductiveCertificatePFXHex;
        }
        public void InitializeUninitializedValues()
        {
            if(this.ProgramFolder == null)
            {
                this.ProgramFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            }
            if(this.BaseFolder == null)
            {
                this.BaseFolder = this.ExecutionMode.Accept(new GetBaseFolder(this.TargetEnvironmentType, this.ProgramFolder));
            }
            if(this.ConfigurationFolder == null)
            {
                this.ConfigurationFolder = Path.Combine(this.BaseFolder, "Configuration");
            }
            if(this.LogFolder == null)
            {
                this.LogFolder = Path.Combine(this.BaseFolder, "Log");
            }
            if(this.LogFile == null)
            {
                this.LogFile = Path.Join(this.LogFolder, $"{this.AppName}.Core.log");
            }
            if(this.CertificateFolder == null)
            {
                this.CertificateFolder = Path.Combine(this.ConfigurationFolder, "Certificate");
            }
            if(this.UseHTTPS)
            {
                this.TLSCertificatePFXFilePath = Path.Join(this.CertificateFolder, $"{this.Domain}.pfx");
                this.TLSCertificatePasswordFile = Path.Join(this.CertificateFolder, $"{this.Domain}.password");
            }
            if(this.PreRun == null)
            {
                this.PreRun = () => { };
            }
            if(this.PostRun == null)
            {
                this.PostRun = () => { };
            }
            if(this.initializeServices == null)
            {
                this.initializeServices = (services) => { };
            }
        }
    }
}
