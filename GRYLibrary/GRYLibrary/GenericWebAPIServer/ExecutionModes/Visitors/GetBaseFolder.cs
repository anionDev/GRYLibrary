using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.ExecutionModes.Visitors
{
    public class GetBaseFolder : IExecutionModeVisitor<string>
    {
        private readonly GRYEnvironment _TargetEnvironmentType;
        private readonly string _ProgramFolder;
        public GetBaseFolder(GRYEnvironment targetEnvironmentType, string programFolder)
        {
            this._TargetEnvironmentType = targetEnvironmentType;
            this._ProgramFolder = programFolder;
        }
        public string Handle(Analysis analysis)
        {
            var result=Path.Join(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            GRYLibrary.Core.Miscellaneous.Utilities.EnsureDirectoryExists(result);
            return result;
        }

        public string Handle(RunProgram runProgram)
        {
            return GenericWebAPIServer.GetBaseFolderForProjectInCommonProjectStructure(_TargetEnvironmentType, _ProgramFolder);
        }
    }
}
