
using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.Miscellaneous;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IInjectableSettings
    {
        public string Address { get; }
        public string Domain { get; }
        public ExecutionMode ExecutionMode { get; }
        public GRYEnvironment TargetEnvironmentType { get; }
        public string ProgramFolder { get; }
        public string AppName { get; }
        public Version3 AppVersion { get; }
        public string LogFolder { get; }
    }
    public class InjectableSettings :IInjectableSettings
    {
        public string Address { get; }
        public string Domain { get; }
        public ExecutionMode ExecutionMode { get; }
        public GRYEnvironment TargetEnvironmentType { get; }
        public string ProgramFolder { get; }
        public string AppName { get; }
        public Version3 AppVersion { get; }
        public string LogFolder { get; }
        public InjectableSettings(string address, string domain, ExecutionMode executionMode, GRYEnvironment targetEnvironmentType, string programFolder, string appName, Version3 appVersion, string logFolder)
        {
            this.Address = address;
            this.Domain = domain;
            this.ExecutionMode = executionMode;
            this.TargetEnvironmentType = targetEnvironmentType;
            this.ProgramFolder = programFolder;
            this.AppName = appName;
            this.AppVersion = appVersion;
            this.LogFolder = logFolder;
        }

    }
}
