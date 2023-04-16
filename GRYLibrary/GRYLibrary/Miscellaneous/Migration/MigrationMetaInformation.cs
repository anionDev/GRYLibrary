using System;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class MigrationMetaInformation
    {
     public  Version3 SourceVersion { get; set; }
     public  Version3 TargetVersion { get; set; }
     public  Action<MigrationInformation> Migration{ get; set; }
}
}
