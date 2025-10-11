﻿using CommandLine;
using GRYLibrary.Core.Misc.ConsoleApplication;

namespace GRYLibrary.Core.APIServer.Verbs
{
    public interface IAPIServerCommandlineParameter : ICommandlineParameter
    {
        public bool RealRun { get; set; }
    }
}
