using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using System;
using System.IO;

namespace GRYLibrary.Core.GenericWebAPIServer.ExecutionModes.Visitors
{
    public class GetBaseFolder :IExecutionModeVisitor<string>
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
            string result = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            Miscellaneous.Utilities.EnsureDirectoryExists(result);
            return result;
        }

        public string Handle(RunProgram runProgram)
        {
            return GetBaseFolderForProjectInCommonProjectStructure(this._TargetEnvironmentType, this._ProgramFolder);
        }

        public static string GetBaseFolderForProjectInCommonProjectStructure(GRYEnvironment environment, string programFolder)
        {
            string workspaceFolderName = "Workspace";
            if(environment is Development)
            {
                return Miscellaneous.Utilities.ResolveToFullPath($"../../{workspaceFolderName}", programFolder);
            }
            else
            {
                return $"/{workspaceFolderName}";
            }
        }
    }
}