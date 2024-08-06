using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using System;
using System.IO;

namespace GRYLibrary.Core.APIServer.ExecutionModes.Visitors
{
    public class GetBaseFolder : IExecutionModeVisitor<string>
    {
        private readonly GRYEnvironment _TargetEnvironmentType;
        private readonly ExecutionMode _ExecutionMode;
        private readonly string _ProgramFolder;
        public GetBaseFolder(GRYEnvironment targetEnvironmentType, string programFolder, ExecutionMode executionMode)
        {
            this._TargetEnvironmentType = targetEnvironmentType;
            this._ExecutionMode = executionMode;
            this._ProgramFolder = programFolder;
        }

        public string Handle(RunProgram runProgram)
        {
            return GetBaseFolderForProjectInCommonProjectStructure(this._TargetEnvironmentType, this._ProgramFolder, this._ExecutionMode);
        }

        public string Handle(TestRun testRun)
        {
            return this.GetTempFolder();
        }
        public string Handle(Analysis analysis)
        {
            return this.GetTempFolder();
        }

        private string GetTempFolder()
        {
            string result = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            Misc.Utilities.EnsureDirectoryExists(result);
            return result;
        }

        public static string GetBaseFolderForProjectInCommonProjectStructure(GRYEnvironment environment, string programFolder, ExecutionMode executionMode)
        {
            string workspaceFolderName = "Workspace";
            string result;
            if (environment is Development || executionMode is not RunProgram)
            {
                result = Misc.Utilities.ResolveToFullPath($"../../{workspaceFolderName}", programFolder);//runing locally
            }
            else
            {
                result = $"/{workspaceFolderName}";//running in container
            }
            return result;
        }
    }
}