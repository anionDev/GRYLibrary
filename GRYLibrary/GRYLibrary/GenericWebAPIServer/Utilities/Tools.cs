using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.Migration;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GRYLibrary.Core.GenericWebAPIServer.GenericWebAPIServer;

namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public static class Tools
    {

        public static int WebAPIMainMethod<PersistedConfiguration, CommandlineParameter>(CommandlineParameter commandlineArguments, ExecutionMode executionMode, GeneralWebAPIConfigInitializationInformation config) where PersistedConfiguration : WebAPIConfigurationVariables, new()
        {
            GRYEnvironment targetEnvironmentType = config.TargetEnvironmentType;
            string domain = config.Domain;
            string programFolder = config.ProgramFolder;
            string baseFolder = config.BaseFolder;
            string configurationFolder = config.ConfigurationFolder;
            string logFolder = config.LogFolder;
            string logFile = config.LogFile;
            string certificateFolder = config.CertificateFolder;
            try
            {
                GRYLogConfiguration logConfiguration = GenericWebAPIServer.GetDefaultLogConfiguration(logFile, true, targetEnvironmentType);
                IGeneralLogger logger = executionMode.Accept(new GetLoggerVisitor(logConfiguration));
                GRYMigration.MigrateIfRequired(config.AppName, config.AppVersion, logger, baseFolder, targetEnvironmentType, executionMode, new Dictionary<object, object>(), new HashSet<MigrationMetaInformation>());

                string protocol = config.UseHTTPS ? "https" : "http";
                if(config.UseHTTPS && !File.Exists(config.TLSCertificatePFXFilePath))
                {
                    if(targetEnvironmentType is Productive)
                    {
                        logger.Log($"\"{config.TLSCertificatePFXFilePath}\" does not exist. Attempt to retrieve nonproductive-certificate. It is recommended to replace it by a productive-certificate as soon as possible.", LogLevel.Warning);
                    }
                    Miscellaneous.Utilities.EnsureFileExists(config.TLSCertificatePasswordFile, true);
                    File.WriteAllBytes(config.TLSCertificatePasswordFile, Miscellaneous.Utilities.HexStringToByteArray(config.NonProductiveCertificatePasswordHex));
                    Miscellaneous.Utilities.EnsureFileExists(config.TLSCertificatePFXFilePath, true);
                    File.WriteAllBytes(config.TLSCertificatePFXFilePath, Miscellaneous.Utilities.HexStringToByteArray(config.NonProductiveCertificatePFXHex));
                }
                GeneralApplicationSettings generalApplicationSettings = new GeneralApplicationSettings()
                {
                    Enabled = true,
                    TermsOfServiceURL = $"{protocol}://{config.Domain}/Other/TermsOfService",
                    ContactURL = $"{protocol}://{config.Domain}/Other/Contact",
                    LicenseURL = $"{protocol}://{config.Domain}/Other/License",
                    AppDescription = config.AppDescription,
                    LogConfiguration = logConfiguration
                };
                WebServerSettings webServerSettings = new WebServerSettings()
                {
                    Port = (ushort)(config.UseHTTPS ? 443 : 80),
                    TLSCertificatePasswordFile = config.TLSCertificatePasswordFile,
                    TLSCertificatePFXFilePath = config.TLSCertificatePFXFilePath,
                    BlackListProvider = new BlacklistProvider(),
                    DDOSProtectionSettings = new DDOSProtectionSettings(),
                    ObfuscationSettings = new ObfuscationSettings(),
                    ExceptionManagerSettings = new ExceptionManagerSettings(),
                    RequestCounterSettings = new RequestCounterSettings(),
                    RequestLoggingSettings = new RequestLoggingSettings() { WebServerAccessLogFile = Path.Join(logFolder, $"{config.AppName}.WebServerAccess.log") },
                    WebApplicationFirewallSettings = new WebApplicationFirewallSettings(),
                    APIKeyValidatorSettings = new APIKeyValidatorSettings(),
                };
                CustomWebAPIConfigurationVariables<PersistedConfiguration> webAPIConfigurationVariables = new CustomWebAPIConfigurationVariables<PersistedConfiguration>()
                {
                    GeneralApplicationSettings = generalApplicationSettings,
                    WebServerSettings = webServerSettings,
                    ApplicationSpecificSettings = new PersistedConfiguration() { }
                };
                WebAPIConfigurationConstants webAPIConfigurationConstants = new WebAPIConfigurationConstants(
                    targetEnvironmentType,
                    config.AppName,
                    config.AppVersion.ToString(),
                    Path.Combine(configurationFolder, "WebAPIServerSettings.xml"),
                    (serviceCollection) => config.initializeServices(serviceCollection)
                );
                WebAPIConfigurationValues<WebAPIConfigurationConstants, CustomWebAPIConfigurationVariables<PersistedConfiguration>> webAPIConfigurationValues = new WebAPIConfigurationValues<WebAPIConfigurationConstants, CustomWebAPIConfigurationVariables<PersistedConfiguration>>()
                {
                    WebAPIConfigurationConstants = webAPIConfigurationConstants,
                    WebAPIConfigurationVariables = webAPIConfigurationVariables,
                    RethrowInitializationExceptions = true,
                    CommandlineArguments = Array.Empty<string>(),
                    ExecutionMode = executionMode
                };
                WebAPIConfiguration<WebAPIConfigurationConstants, CustomWebAPIConfigurationVariables<PersistedConfiguration>> webAPIConfiguration = new WebAPIConfiguration<WebAPIConfigurationConstants, CustomWebAPIConfigurationVariables<PersistedConfiguration>>()
                {
                    WebAPIConfigurationValues = webAPIConfigurationValues,
                    Logger = logger
                };
                webAPIConfiguration.WebAPIConfigurationValues.ExecutionMode.Accept(new GenericWebAPIServer.CreateFolderVisitor(baseFolder, configurationFolder, certificateFolder));
                int result = GenericWebAPIServer.DefaultWebAPIMainFunction<WebAPIConfigurationConstants, CustomWebAPIConfigurationVariables<PersistedConfiguration>>(webAPIConfiguration, config.PreRun, config.PostRun);
                return result;
            }
            finally
            {
                if(executionMode is Analysis)
                {
                    GRYLibrary.Core.Miscellaneous.Utilities.EnsureDirectoryDoesNotExist(baseFolder);
                }
            }
        }
    }
}
