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
using GRYLibrary.Core.Miscellaneous.FilePath;

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
            RelativeFilePath configurationFolder = new RelativeFilePath() { FilePath = initialInformation.ConfigurationFolderRelative };
            RelativeFilePath logFolder = new RelativeFilePath() { FilePath = initialInformation.LogFolderRelative };
            RelativeFilePath logFile = new RelativeFilePath() { FilePath = initialInformation.LogFileRelative };
            RelativeFilePath certificateFolder = new RelativeFilePath() { FilePath = initialInformation.CertificateFolderRelative };
            RelativeFilePath tlsCertificatePFXFile = new RelativeFilePath() { FilePath = initialInformation.TLSCertificatePFXFilePathRelative };
            RelativeFilePath tlsCertificatePasswordFile = new RelativeFilePath() { FilePath = initialInformation.TLSCertificatePasswordFileRelative };
            try
            {
                GRYLogConfiguration logConfiguration = GetDefaultLogConfiguration(logFile, true, targetEnvironmentType);
                logger = executionMode.Accept(new GetLoggerVisitor(logConfiguration, baseFolder));
                GRYMigration.MigrateIfRequired(initialInformation.AppName, initialInformation.AppVersion, logger, baseFolder, targetEnvironmentType, executionMode, new Dictionary<object, object>(), new HashSet<MigrationMetaInformation>());

                string protocol = initialInformation.UseHTTPS ? "https" : "http";
                if(initialInformation.UseHTTPS && !File.Exists(tlsCertificatePFXFile.GetPath(baseFolder)))
                {
                    if(targetEnvironmentType is Productive)
                    {
                        logger.Log($"\"{tlsCertificatePFXFile.GetPath(baseFolder)}\" does not exist. Attempt to retrieve nonproductive-certificate. It is recommended to replace it by a productive-certificate as soon as possible.", LogLevel.Warning);
                    }
                    Miscellaneous.Utilities.EnsureFileExists(tlsCertificatePasswordFile.GetPath(baseFolder), true);
                    File.WriteAllBytes(tlsCertificatePasswordFile.GetPath(baseFolder), Miscellaneous.Utilities.HexStringToByteArray(initialInformation.NonProductiveCertificatePasswordHex));
                    Miscellaneous.Utilities.EnsureFileExists(tlsCertificatePFXFile.GetPath(baseFolder), true);
                    File.WriteAllBytes(tlsCertificatePFXFile.GetPath(baseFolder), Miscellaneous.Utilities.HexStringToByteArray(initialInformation.NonProductiveCertificatePFXHex));
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
                    TLSCertificatePasswordFile = tlsCertificatePasswordFile,
                    TLSCertificatePFXFilePath = tlsCertificatePFXFile,
                    BlackListProvider = new BlacklistProvider(),
                    DDOSProtectionSettings = new DDOSProtectionSettings(),
                    ObfuscationSettings = new ObfuscationSettings(),
                    ExceptionManagerSettings = new ExceptionManagerSettings(),
                    RequestCounterSettings = new RequestCounterSettings(),
                    RequestLoggingSettings = new RequestLoggingSettings() { WebServerAccessLogFile = AbstractFilePath.FromString(logFolder.FilePath + Path.DirectorySeparatorChar + $"{initialInformation.AppName}.WebServerAccess.log") },
                    WebApplicationFirewallSettings = new WebApplicationFirewallSettings(),
                    APIKeyValidatorSettings = new APIKeyValidatorSettings(),
                };
                WebAPIConfigurationConstants webAPIConfigurationConstants = new WebAPIConfigurationConstants(
                     targetEnvironmentType,
                     initialInformation.AppName,
                     initialInformation.AppVersion.ToString(),
                     Path.Combine(configurationFolder.GetPath(baseFolder), "WebAPIServerSettings.xml"),
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
                    ExecutionMode = executionMode,
                    PreRun = initialInformation.PreRun,
                    PostRun = initialInformation.PostRun,
                };
                webAPIConfiguration.ExecutionMode.Accept(new CreateFolderVisitor(baseFolder, configurationFolder.GetPath(baseFolder), certificateFolder.GetPath(baseFolder)));
                int result = WebAPIRunner(webAPIConfiguration);
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
