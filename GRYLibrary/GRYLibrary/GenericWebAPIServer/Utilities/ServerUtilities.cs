using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;
using System.Reflection;

namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public class ServerUtilities
    {

        public static ExecutionMode GetExecutionMode()
        {
            if(Assembly.GetEntryAssembly().GetName().Name == "dotnet-swagger")
            {
                return Analysis.Instance;
            }
            return RunProgram.Instance;
        }
        public static GRYLogConfiguration GetLogConfiguration(string filename, GRYEnvironment environment)
        {
            GRYLogConfiguration result = GRYLogConfiguration.GetCommonConfiguration(AbstractFilePath.FromString("./" + filename), environment is Development);
            foreach(GRYLogTarget target in result.LogTargets)
            {
                target.Format = GRYLogLogFormat.GRYLogFormat;
            }
            return result;
        }
    }
}
