﻿using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes.Visitors;
using GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents a container for all information which are required to start a WebAPI-server.
    /// </summary>
    public class APIServerInitializer<AppSpecificConstants, PersistedApplicationSpecificConfiguration>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public string BaseFolder { get; set; }
        public IApplicationConstants<AppSpecificConstants> ApplicationConstants { get; set; }
        public PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> InitialApplicationConfiguration { get; set; }
        public Action<IServiceCollection, IApplicationConstants<AppSpecificConstants>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>> ConfigureServices { get; set; }
        public Action PreRun { get; set; }
        public Action PostRun { get; set; }
        public AbstractFilePath BasicInformationFile { get; set; }
        public APIServerInitializer(string applicationName, Version3 applicationVersion, ExecutionMode executionMode, GRYEnvironment environment, AppSpecificConstants applicationSpecificConstants, string domain, string appDescription, PersistedApplicationSpecificConfiguration persistedApplicationSpecificConfiguration, string fallbackCertificatePasswordFileContentHex, string fallbackCertificatePFXFileContentHex, Func<string/*apiKey*/, string/*method*/, string/*route*/, bool> apiKeyIsValid)
        {
            this.ApplicationConstants = new ApplicationConstants<AppSpecificConstants>(applicationName, appDescription, applicationVersion, executionMode, environment, applicationSpecificConstants);
            this.BaseFolder = GetDefaultBaseFolder(this.ApplicationConstants);
            this.InitialApplicationConfiguration = PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>.Create(domain, persistedApplicationSpecificConfiguration, environment, fallbackCertificatePasswordFileContentHex, fallbackCertificatePFXFileContentHex,apiKeyIsValid);
            this.ConfigureServices = (_, _, _) => { };
            this.PreRun = () => { };
            this.PostRun = () => { };
            this.BasicInformationFile = AbstractFilePath.FromString("./BasicApplicationInformation.xml");
        }

        internal static string GetDefaultBaseFolder(IApplicationConstants<AppSpecificConstants> applicationConstants)
        {
            string programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string result = applicationConstants.ExecutionMode.Accept(new GetBaseFolder(applicationConstants.Environment, programFolder));
            return result;
        }
    }
}
