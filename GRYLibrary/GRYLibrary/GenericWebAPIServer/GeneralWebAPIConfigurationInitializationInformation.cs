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
    public class GeneralWebAPIConfigurationInitializationInformation
    {
        //<required>
        public string AppName { get; set; }
        public Version3 AppVersion { get; set; }
        public string AppDescription { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment TargetEnvironmentType { get; set; }
        public string Domain { get; set; }
        public bool UseHTTPS { get; set; }

        public string NonProductiveCertificatePasswordHex { get; set; }
        /// <summary>
        /// Represents a certificate which should be used as fallback if <see cref="UseHTTPS"/>==true and the certificate given by <see cref="TLSCertificatePFXFilePathRelative"/> is not available.
        /// </summary>
        public string NonProductiveCertificatePFXHex { get; set; }
        //</required>

        //<optional>
        public string ProgramFolder { get; set; }
        public string BaseFolder { get; set; }
        public string ConfigurationFolderRelative { get; set; }
        public string LogFolderRelative { get; set; }
        public string LogFileRelative { get; set; }
        public string CertificateFolderRelative { get; set; }
        public string TLSCertificatePFXFilePathRelative { get; set; }
        public string TLSCertificatePasswordFileRelative { get; set; }
        public Action PreRun { get; set; }
        public Action PostRun { get; set; }
        public Action<IServiceCollection> InitializeServices { get; set; }
        //</optional>

        public GeneralWebAPIConfigurationInitializationInformation(string appName, Version3 appVersion, string appDescription, ExecutionMode executionMode, GRYEnvironment targetEnvironmentType, string domain, bool useHTTPS, string nonProductiveCertificatePasswordHex, string nonProductiveCertificatePFXHex)
        {
            this.AppName = appName;
            this.AppVersion = appVersion;
            this.AppDescription = appDescription;
            this.ExecutionMode = executionMode;
            this.TargetEnvironmentType = targetEnvironmentType;
            this.Domain = (targetEnvironmentType is Development) ? "localhost" : domain;
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
            if(this.ConfigurationFolderRelative == null)
            {
                this.ConfigurationFolderRelative = "."/*will be replaced by BaseFolder*/ + Path.DirectorySeparatorChar + "Configuration";
            }
            if(this.LogFolderRelative == null)
            {
                this.LogFolderRelative = "."/*will be replaced by BaseFolder*/ + Path.DirectorySeparatorChar + "Log";
            }
            if(this.LogFileRelative == null)
            {
                this.LogFileRelative = this.LogFolderRelative + Path.DirectorySeparatorChar + $"{this.AppName}.Core.log";
            }
            if(this.CertificateFolderRelative == null)
            {
                this.CertificateFolderRelative = this.ConfigurationFolderRelative + Path.DirectorySeparatorChar + "Certificate";
            }
            if(this.UseHTTPS)
            {
                this.TLSCertificatePFXFilePathRelative = this.CertificateFolderRelative + Path.DirectorySeparatorChar + $"{this.Domain}.pfx";
                this.TLSCertificatePasswordFileRelative = this.CertificateFolderRelative + Path.DirectorySeparatorChar + $"{this.Domain}.password";
            }
            else
            {
                this.TLSCertificatePFXFilePathRelative = null;
                this.TLSCertificatePasswordFileRelative = null;
            }
            if(this.PreRun == null)
            {
                this.PreRun = () => { };
            }
            if(this.PostRun == null)
            {
                this.PostRun = () => { };
            }
            if(this.InitializeServices == null)
            {
                this.InitializeServices = (services) => { };
            }
        }
    }
}
