using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using System;
using System.IO;

namespace GRYLibrary.Core.APIServer.ExecutionModes.Visitors
{
    public class GetBaseFolder : IExecutionModeVisitor<string>
    {
        private readonly GRYEnvironment _TargetEnvironmentType;
        private readonly string _ProgramFolder;
        private bool _IsTestRun;
        public GetBaseFolder(GRYEnvironment targetEnvironmentType, string programFolder, bool isTestRun)
        {
            this._TargetEnvironmentType = targetEnvironmentType;
            this._ProgramFolder = programFolder;
            this._IsTestRun = isTestRun;
        }

        public string Handle(RunProgram runProgram)
        {
            return GetBaseFolderForProjectInCommonProjectStructure(this._TargetEnvironmentType, this._ProgramFolder, runProgram, this._IsTestRun);
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

        public static string GetBaseFolderForProjectInCommonProjectStructure(GRYEnvironment environment, string programFolder, ExecutionMode executionMode, bool isTestRun)
        {
            string workspaceFolderName = "Workspace";
            string result;
            if (environment is Development || executionMode is not RunProgram|| isTestRun)
            {
                result = Misc.Utilities.ResolveToFullPath($"../../{workspaceFolderName}", programFolder);//running locally
            }
            else
            {
                result = $"/{workspaceFolderName}";//running in container
            }
            return result;
        }
    }
}