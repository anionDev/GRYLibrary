using System;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class AdministrationSettings : IAdministrationSettings
    {
        /// <inheritdoc/>
        public string ProgramName { get; }
        /// <inheritdoc/>
        public Version ProgramVersion { get; }
        /// <inheritdoc/>
        public IEnvironment Environment { get; }
        public string ConfigurationFolder { get; }

        public AdministrationSettings(string programName, Version programVersion, IEnvironment environment, string configurationFolder)
        {
            ProgramName = programName;
            ProgramVersion = programVersion;
            Environment = environment;
            ConfigurationFolder = configurationFolder;
        }
    }
}
