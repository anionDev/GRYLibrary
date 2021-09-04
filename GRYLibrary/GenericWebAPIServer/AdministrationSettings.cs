using System;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class AdministrationSettings : IAdministrationSettings
    {
        /// <inheritdoc/>
        public string ProgramName { get; set; }
        /// <inheritdoc/>
        public Version ProgramVersion { get; set; }
        /// <inheritdoc/>
        public IEnvironment Environment { get; set; }
        public AdministrationSettings(string programName, Version programVersion, IEnvironment environment)
        {
            ProgramName = programName;
            ProgramVersion = programVersion;
            Environment = environment;
        }
    }
}
