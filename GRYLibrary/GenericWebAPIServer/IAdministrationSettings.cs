using System;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface IAdministrationSettings
    {
        public string ProgramName { get; }
        public Version ProgramVersion { get;  }
        public IEnvironment Environment { get;  }
        public string ConfigurationFolder { get; }
    }
}
