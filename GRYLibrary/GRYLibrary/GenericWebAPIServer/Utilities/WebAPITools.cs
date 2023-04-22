using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.Migration;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using static GRYLibrary.Core.GenericWebAPIServer.GenericWebAPIServer;
using GRYLibrary.Core.Miscellaneous.ConsoleApplication;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public static class WebAPITools
    {
        public static int WebAPIMainMethod<PersistedConfigurationType, CommandlineParameterType>(CommandlineParameterType commandlineArguments, ExecutionMode executionMode, GeneralWebAPIConfigurationInitializationInformation initialInformation)
            where PersistedConfigurationType : WebAPIConfigurationVariables, new()
            where CommandlineParameterType : ICommandlineParameter
        {
            IGeneralLogger logger = GeneralLogger.CreateUsingConsole();
            GRYEnvironment targetEnvironmentType = initialInformation.TargetEnvironmentType;
            string domain = initialInformation.Domain;
            string programFolder = initialInformation.ProgramFolder;
            string baseFolder = initialInformation.BaseFolder;
            string configurationFolder = initialInformation.ConfigurationFolder;
            string logFolder = initialInformation.LogFolder;
            string logFile = initialInformation.LogFile;
            string certificateFolder = initialInformation.CertificateFolder;
            try
            {
                GRYLogConfiguration logConfiguration = GetDefaultLogConfiguration(logFile, true, targetEnvironmentType);
                logger = executionMode.Accept(new GetLoggerVisitor(logConfiguration));
                GRYMigration.MigrateIfRequired(initialInformation.AppName, initialInformation.AppVersion, logger, baseFolder, targetEnvironmentType, executionMode, new Dictionary<object, object>(), new HashSet<MigrationMetaInformation>());

                string protocol = initialInformation.UseHTTPS ? "https" : "http";
                if(initialInformation.UseHTTPS && !File.Exists(initialInformation.TLSCertificatePFXFilePath))
                {
                    if(targetEnvironmentType is Productive)
                    {
                        logger.Log($"\"{initialInformation.TLSCertificatePFXFilePath}\" does not exist. Attempt to retrieve nonproductive-certificate. It is recommended to replace it by a productive-certificate as soon as possible.", LogLevel.Warning);
                    }
                    Miscellaneous.Utilities.EnsureFileExists(initialInformation.TLSCertificatePasswordFile, true);
                    File.WriteAllBytes(initialInformation.TLSCertificatePasswordFile, Miscellaneous.Utilities.HexStringToByteArray(initialInformation.NonProductiveCertificatePasswordHex));
                    Miscellaneous.Utilities.EnsureFileExists(initialInformation.TLSCertificatePFXFilePath, true);
                    File.WriteAllBytes(initialInformation.TLSCertificatePFXFilePath, Miscellaneous.Utilities.HexStringToByteArray(initialInformation.NonProductiveCertificatePFXHex));
                }
                GeneralApplicationSettings generalApplicationSettings = new GeneralApplicationSettings()
                {
                    Enabled = true,
                    TermsOfServiceURL = $"{protocol}://{initialInformation.Domain}/Other/TermsOfService",
                    ContactURL = $"{protocol}://{initialInformation.Domain}/Other/Contact",
                    LicenseURL = $"{protocol}://{initialInformation.Domain}/Other/License",
                    AppDescription = initialInformation.AppDescription,
                    LogConfiguration = logConfiguration
                };
                WebServerConfiguration webServerSettings = new WebServerConfiguration()
                {
                    Port = (ushort)(initialInformation.UseHTTPS ? 443 : 80),
                    TLSCertificatePasswordFile = initialInformation.TLSCertificatePasswordFile,
                    TLSCertificatePFXFilePath = initialInformation.TLSCertificatePFXFilePath,
                    BlackListProvider = new BlacklistProvider(),
                    DDOSProtectionSettings = new DDOSProtectionSettings(),
                    ObfuscationSettings = new ObfuscationSettings(),
                    ExceptionManagerSettings = new ExceptionManagerSettings(),
                    RequestCounterSettings = new RequestCounterSettings(),
                    RequestLoggingSettings = new RequestLoggingSettings() { WebServerAccessLogFile = Path.Join(logFolder, $"{initialInformation.AppName}.WebServerAccess.log") },
                    WebApplicationFirewallSettings = new WebApplicationFirewallSettings(),
                    APIKeyValidatorSettings = new APIKeyValidatorSettings(),
                };
                WebAPIConfigurationConstants webAPIConfigurationConstants = new WebAPIConfigurationConstants(
                     targetEnvironmentType,
                     initialInformation.AppName,
                     initialInformation.AppVersion.ToString(),
                     Path.Combine(configurationFolder, "WebAPIServerSettings.xml"),
                     (serviceCollection) => initialInformation.InitializeServices(serviceCollection)
                 );
                CustomWebAPIConfigurationVariables<PersistedConfigurationType> webAPIConfigurationVariables = new CustomWebAPIConfigurationVariables<PersistedConfigurationType>()
                {
                    GeneralApplicationSettings = generalApplicationSettings,
                    WebServerSettings = webServerSettings,
                    ApplicationSpecificSettings = new PersistedConfigurationType() { }
                };
                WebAPIConfiguration<WebAPIConfigurationConstants, CustomWebAPIConfigurationVariables<PersistedConfigurationType>> webAPIConfiguration = new WebAPIConfiguration<WebAPIConfigurationConstants, CustomWebAPIConfigurationVariables<PersistedConfigurationType>>()
                {
                    Logger = logger,
                    WebAPIConfigurationConstants = webAPIConfigurationConstants,
                    WebAPIConfigurationVariables = webAPIConfigurationVariables,
                    RethrowInitializationExceptions = true,
                    CommandlineArguments = commandlineArguments.OriginalArguments,
                    ExecutionMode = executionMode
                };
                webAPIConfiguration.ExecutionMode.Accept(new CreateFolderVisitor(baseFolder, configurationFolder, certificateFolder));
                int result = WebAPIRunnter(webAPIConfiguration, initialInformation.PreRun, initialInformation.PostRun);
                return result;
            }
            catch(Exception exception)
            {
                logger.LogException(exception, "Fatal error occurred while initalizing or running the WebAPIServer");
                return 1;
            }
            finally
            {
                if(executionMode is Analysis)
                {
                    Miscellaneous.Utilities.EnsureDirectoryDoesNotExist(baseFolder);
                }
            }
        }
    }
}
