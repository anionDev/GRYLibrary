﻿using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;

namespace GRYLibrary.Core.Miscellaneous.ConsoleApplication
{
    public class GRYConsoleApplicationInitialInformation
    {
        public string ProgramName { get; private set; }
        public string ProgramVersion { get; private set; }
        public string ProgramDescription { get; private set; }
        public ExecutionMode ExecutionMode { get; private set; }
        public GRYEnvironment Environment { get; private set; }

        public GRYConsoleApplicationInitialInformation(string programName, string programVersion, string programDescription, ExecutionMode executionMode, GRYEnvironment environment)
        {
            this.ProgramName = programName;
            this.ProgramVersion = programVersion;
            this.ProgramDescription = programDescription;
            this.ExecutionMode = executionMode;
            this.Environment = environment;
        }
    }
}
