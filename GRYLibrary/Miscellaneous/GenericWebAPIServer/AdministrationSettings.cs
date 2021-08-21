﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer
{
    public class AdministrationSettings : IAdministrationSettings
    {
        public string ProgramName { get; set; }
        public Version Version { get; set; }
        public IEnvironment Environment { get; set; }
        public AdministrationSettings(string programName, Version version, IEnvironment environment)
        {
            ProgramName = programName;
            Version = version;
            Environment = environment;
        }
    }
}
