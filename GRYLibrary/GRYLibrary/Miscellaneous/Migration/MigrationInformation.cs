using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class MigrationInformation
    {
     public  Version3 BaseVersion { get;private set; }
     public  string BaseFolder { get; private set; }
     public  GRYEnvironment TargetEnvironmentType { get; private set; }
     public  ExecutionMode ExecutionMode { get; private set; }
     public  IDictionary<object, object> CustomValues { get; private set; }

        public MigrationInformation(Version3 baseVersion, string baseFolder, GRYEnvironment targetEnvironmentType, ExecutionMode executionMode, IDictionary<object, object> customValues)
        {
            this.BaseVersion = baseVersion;
            this.BaseFolder = baseFolder;
            this.TargetEnvironmentType = targetEnvironmentType;
            this.ExecutionMode = executionMode;
            this.CustomValues = customValues;
        }
    }
}
