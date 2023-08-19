using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.ExecutionModes.Visitors;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.ConsoleApplication;
using GRYLibrary.Core.Miscellaneous.FilePath;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GRYLibrary.Core.APIServer.Settings
{
    public class APIServerInitializer
    {
        public static APIServerInitializer<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType> Create<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType>(GRYConsoleApplicationInitialInformation gryConsoleApplicationInitialInformation)
              where PersistedAppSpecificConfiguration : new()
              where AppSpecificConstants : new()
              where CommandlineParameterType : ICommandlineParameter
        {
            string applicationName = gryConsoleApplicationInitialInformation.ProgramName;
            string appDescription = gryConsoleApplicationInitialInformation.ProgramDescription;
            Version3 applicationVersion = Version3.Parse(gryConsoleApplicationInitialInformation.ProgramVersion);
            ExecutionMode executionMode = gryConsoleApplicationInitialInformation.ExecutionMode;
            GRYEnvironment environment = gryConsoleApplicationInitialInformation.Environment;
            AppSpecificConstants applicationSpecificConstants = new AppSpecificConstants();
            string domain = default;//TODO set value (this value can probably be derived by the environment-variable and a development-certificate-name)
            string fallbackCertificatePasswordFileContentHex = default;//TODO set value and make sure it is not a fallback anymore
            string fallbackCertificatePFXFileContentHex = default;//TODO set value and make sure it is not a fallback anymore
            APIServerInitializer<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType> result = new APIServerInitializer<AppSpecificConstants, PersistedAppSpecificConfiguration, CommandlineParameterType>
            {
                ApplicationConstants = new ApplicationConstants<AppSpecificConstants>(applicationName, appDescription, applicationVersion, executionMode, environment, applicationSpecificConstants)
            };
            PersistedAppSpecificConfiguration initialConfig = new PersistedAppSpecificConfiguration();
            result.ApplicationConstants.KnownTypes.Add(typeof(PersistedAppSpecificConfiguration));
            result.InitialApplicationConfiguration = PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>.Create(domain, initialConfig, environment, fallbackCertificatePasswordFileContentHex, fallbackCertificatePFXFileContentHex, result.ApplicationConstants.ApplicationName);
            result.BaseFolder = GetDefaultBaseFolder(result.ApplicationConstants);
            result.ApplicationConstants.Initialize(result.BaseFolder);
            result.PreRun = () => { };
            result.PostRun = () => { };
            result.Filter = new HashSet<FilterDescriptor>();
            result.BasicInformationFile = AbstractFilePath.FromString("./BasicApplicationInformation.xml");
            return result;
        }
        private static string GetDefaultBaseFolder<AppConstantsType>(IApplicationConstants<AppConstantsType> applicationConstants)
        {
            string programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return applicationConstants.ExecutionMode.Accept(new GetBaseFolder(applicationConstants.Environment, programFolder));
        }
        #region Create or load config-file

        internal static IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> LoadConfiguration<PersistedAppSpecificConfiguration>(
            ISet<Type> knownTypes, GRYEnvironment evironment, ExecutionMode executionMode, string configurationFile, bool throwErrorIfConfigurationDoesNotExistInProduction,
            PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> initialConfiguration)
                where PersistedAppSpecificConfiguration : new()
        {
            if(throwErrorIfConfigurationDoesNotExistInProduction && evironment is Productive && !File.Exists(configurationFile))
            {
                throw new FileNotFoundException($"Configurationfile \"{configurationFile}\" does not exist.");
            }
            else
            {
                IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> result = executionMode.Accept(new GetPersistedAPIServerConfigurationVisitor<PersistedAppSpecificConfiguration>(configurationFile, initialConfiguration, knownTypes));
                return result;
            }
        }
        private class GetPersistedAPIServerConfigurationVisitor<PersistedApplicationSpecificConfiguration> :IExecutionModeVisitor<IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>>
                where PersistedApplicationSpecificConfiguration : new()
        {
            private readonly MetaConfigurationSettings<PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>> _MetaConfiguration;
            private readonly ISet<Type> _KnownTypes;

            public GetPersistedAPIServerConfigurationVisitor(string file, PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> initialValue, ISet<Type> knownTypes)
            {
                this._MetaConfiguration = new MetaConfigurationSettings<PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>>()
                {
                    ConfigurationFormat = XML.Instance,
                    File = file,
                    InitialValue = initialValue
                };
                this._KnownTypes = knownTypes;
            }

            public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> Handle(Analysis analysis)
            {
                return this._MetaConfiguration.InitialValue;
            }

            public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> Handle(RunProgram runProgram)
            {
                return MetaConfigurationManager.GetConfiguration(this._MetaConfiguration, this._KnownTypes);
            }
        }
        #endregion
    }
    /// <summary>
    /// Represents a container for functional information which are required to start a WebAPI-server.
    /// </summary>
    public class APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
    where PersistedApplicationSpecificConfiguration : new()
    {
        public APIServerInitializer() { }
        public string BaseFolder { get; set; }
        public IApplicationConstants<ApplicationSpecificConstants> ApplicationConstants { get; set; }
        /// <summary>
        /// Represents the default-value for the configuration which should be used when there is not already a persisted configuration which can be loaded.
        /// </summary>
        public PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> InitialApplicationConfiguration { get; set; }
        public Action PreRun { get; set; }
        public Action PostRun { get; set; }
        public AbstractFilePath BasicInformationFile { get; set; }
        public ISet<FilterDescriptor> Filter { get; set; }
    }
}
