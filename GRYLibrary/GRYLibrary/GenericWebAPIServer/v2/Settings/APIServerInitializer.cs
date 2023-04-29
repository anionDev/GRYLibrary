using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes.Visitors;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.v2.Settings
{
    /// <summary>
    /// Represents a container for all information which are required to start a WebAPI-server.
    /// </summary>
    public class APIServerInitializer<AppSpecificConstants, PersistedApplicationSpecificConfiguration>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public string BaseFolder { get; set; }
        public IApplicationConstants<AppSpecificConstants> ApplicationConstants { get; set; }
        public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> ApplicationConfiguration { get; set; }
        public Action<ServiceCollection, IApplicationConstants<AppSpecificConstants>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>> ConfigureServices { get; set; }
        public Action PreRun { get; set; }
        public Action PostRun { get; set; }
        public AbstractFilePath ConfigurationFolder { get; set; }
        public AbstractFilePath ConfigurationFile { get; set; }
        public AbstractFilePath BasicInformationFile { get; set; }
        public AbstractFilePath LogFolder { get; set; }
        public AbstractFilePath WebServerLogFile { get; set; }
        public AbstractFilePath WebServerAccessLogFile { get; set; }
        public APIServerInitializer(IApplicationConstants<AppSpecificConstants> applicationConstants, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> applicationConfiguration)
        {
            this.ApplicationConstants = applicationConstants;
            this.ApplicationConfiguration = applicationConfiguration;
            this.BaseFolder = GetDefaultBaseFolder(applicationConstants);
            ConfigurationFolder = RelativeFilePath.FromString("./Configuration");
            ConfigurationFile = RelativeFilePath.FromString("ApplicationConfiguration.xml");
            BasicInformationFile = RelativeFilePath.FromString("./BasicInformation.xml");
            LogFolder = RelativeFilePath.FromString("./Logs");
            WebServerLogFile = RelativeFilePath.FromString("./Application.log");
            WebServerAccessLogFile = RelativeFilePath.FromString("./Access.log");
            ConfigureServices = (_, _, _) => { };
            PreRun = () => { };
            PostRun = () => { };
        }

        public string GetConfigurationFolder() { return ConfigurationFolder.GetPath(BaseFolder); }
        public string GetConfigurationFile() { return ConfigurationFile.GetPath(GetConfigurationFolder()); }
        public string GetBasicInformationFile() { return BasicInformationFile.GetPath(GetConfigurationFolder()); }
        public string GetLogFolder() { return LogFolder.GetPath(BaseFolder); }
        public string GetWebServerLogFile() { return WebServerLogFile.GetPath(GetLogFolder()); }
        public string GetWebServerAccessLogFile() { return WebServerAccessLogFile.GetPath(GetLogFolder()); }
        private string GetDefaultBaseFolder(IApplicationConstants<AppSpecificConstants> applicationConstants)
        {
            var programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var result= applicationConstants.ExecutionMode.Accept(new GetBaseFolder(applicationConstants.Environment, programFolder));
            return result;
        }
    }
}
