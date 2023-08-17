using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.ExecutionModes.Visitors;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GRYLibrary.Core.APIServer.Settings
{
    public class APIServerInitializer
    {
        public static APIServerInitializer<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType> Create<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType>()
              where PersistedAppSpecificConfiguration : new()
        {
            string applicationName = default;//TODO set value (take from "parent"-gryconsole-application-object)
            string appDescription = default;//TODO set value (take from "parent"-gryconsole-application-object)
            Version3 applicationVersion = default;//TODO set value (take from "parent"-gryconsole-application-object)
            ExecutionMode executionMode = default;//TODO set value (take from "parent"-gryconsole-application-object)
            GRYEnvironment environment = default;//TODO set value (take from "parent"-gryconsole-application-object)
            AppSpecificConstants applicationSpecificConstants = default;//TODO set value
            PersistedAppSpecificConfiguration persistedApplicationSpecificConfiguration = default;//TODO set value (load from disk and create from disk before if not exist)
            string domain = default;//TODO set value (this value can probably be derived by the environment-variable and a development-certificate-name)
            string fallbackCertificatePasswordFileContentHex = default;//TODO set value and make sure it is not a fallback anymore
            string fallbackCertificatePFXFileContentHex = default;//TODO set value and make sure it is not a fallback anymore
            APIServerInitializer<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType> result = new APIServerInitializer<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType>
            {
                ApplicationConstants = new ApplicationConstants<AppSpecificConstants>(applicationName, appDescription, applicationVersion, executionMode, environment, applicationSpecificConstants)
            };
            result.ApplicationConstants.KnownTypes.Add(typeof(PersistedAppSpecificConfiguration));
            result.BaseFolder = GetDefaultBaseFolder(result.ApplicationConstants);
            result.InitialApplicationConfiguration = PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>.Create(domain, persistedApplicationSpecificConfiguration, environment, fallbackCertificatePasswordFileContentHex, fallbackCertificatePFXFileContentHex, result.ApplicationConstants.ApplicationName);
            result.ConfigureServices = (_) => { };
            result.PreRun = (_, _) => { };
            result.PostRun = (_, _) => { };
            result.BasicInformationFile = AbstractFilePath.FromString("./BasicApplicationInformation.xml");
            return result;
        }
        private static string GetDefaultBaseFolder<AppConstantsType>(IApplicationConstants<AppConstantsType> applicationConstants)
        {
            string programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return applicationConstants.ExecutionMode.Accept(new GetBaseFolder(applicationConstants.Environment, programFolder));
        }
    }
    /// <summary>
    /// Represents a container for all information which are required to start a WebAPI-server.
    /// </summary>
    public class APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
    where PersistedApplicationSpecificConfiguration : new()
    {
        public bool ThrowErrorIfConfigurationDoesNotExistInProduction { get; set; } = false;
        public string BaseFolder { get; set; }
        public IApplicationConstants<ApplicationSpecificConstants> ApplicationConstants { get; set; }
        public PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> InitialApplicationConfiguration { get; set; }
        public Action<ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>> ConfigureServices { get; set; }
        public Action<IApplicationConstants<ApplicationSpecificConstants>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>> PreRun { get; set; }
        public Action<IApplicationConstants<ApplicationSpecificConstants>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>> PostRun { get; set; }
        public AbstractFilePath BasicInformationFile { get; set; }
        public ISet<FilterDescriptor> Filter { get; set; } = new HashSet<FilterDescriptor>();
    }
}
