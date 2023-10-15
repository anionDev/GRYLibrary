using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class MigrateInstanceInformation
    {
         public IDictionary<object, object> CustomValues { get; set; }
        public ISet<MigrationMetaInformation> MetaMigrations { get; set; }
    }
}
