using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class MigrationConfiguration
    {
        public ISet<MigrationMetaInformation> MigrationMetaInformations { get; set; }
    }
}
