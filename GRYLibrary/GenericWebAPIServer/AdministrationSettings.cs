using System;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class AdministrationSettings : IAdministrationSettings
    {
        /// <inheritdoc/>
        public string ProgramName { get; set; }
        /// <inheritdoc/>
        public Version Version { get; set; }
        /// <inheritdoc/>
        public IEnvironment Environment { get; set; }
        public AdministrationSettings(string programName, Version version, IEnvironment environment)
        {
            ProgramName = programName;
            Version = version;
            Environment = environment;
        }
    }
}
