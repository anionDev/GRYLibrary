using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;

namespace GRYLibrary.Core.Miscellaneous.ConsoleApplication
{
    public class GRYConsoleApplicationInitialInformation
    {
        public string ProgramName { get; set; }
        public string ProgramVersion { get; set; }
        public string ProgramDescription { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment Environment { get; set; }

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
