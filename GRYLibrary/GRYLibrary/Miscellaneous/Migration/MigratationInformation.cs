using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class MigratationInformation
    {
        public IDictionary<object, object> CustomValues { get; set; }
        public Version3 SourceVersion { get; set; }
        public Version3 TargetVersion { get; set; }
    }
}
