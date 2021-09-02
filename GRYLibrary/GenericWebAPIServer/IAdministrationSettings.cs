using System;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface IAdministrationSettings
    {
        public string ProgramName { get; set; }
        public Version Version { get; set; }
        public IEnvironment Environment{ get; set; }
    }
}
