using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.v2.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.v2
{
    internal class APIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
    {
        private readonly APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration> _APIServerInitializer;
        public APIServer(APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration> apiServerInitializer)
        {
            this._APIServerInitializer = apiServerInitializer;
        }
        public int Run(string commandlineArguments)
        {
            CommandlineParameterType commandlineParameter = ParseCommandlineParameter(commandlineArguments);
            IGeneralLogger logger = GeneralLogger.CreateUsingConsole();
            RunMigration();
            PersistedApplicationSpecificConfiguration persistedApplicationSpecificConfiguration = LoadConfiguration();
            _APIServerInitializer.PreRun();
            RunAPIServer();
            _APIServerInitializer.PostRun();
            return 0;
        }

        private CommandlineParameterType ParseCommandlineParameter(string commandlineArguments)
        {
            throw new NotImplementedException();
        }

        private void RunAPIServer()
        {
            //TODO add settings to injectable services
            //TODO run _APIServerInitializer.ConfigureServices()
            //TODO start webserver
            throw new NotImplementedException();
        }

        private PersistedApplicationSpecificConfiguration LoadConfiguration()
        {
            //create configuration if not existent
            throw new NotImplementedException();
        }

        private void RunMigration()
        {
            throw new NotImplementedException();
        }
    }
}
