using System;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface IAdministrationSettings
    {
        public string ProgramName { get; set; }
        public Version ProgramVersion { get; set; }
        public IEnvironment Environment { get; set; }
    }
}
