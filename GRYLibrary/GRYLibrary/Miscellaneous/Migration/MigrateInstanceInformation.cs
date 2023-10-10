using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class MigrateInstanceInformation
    {
         public IDictionary<object, object> CustomValues { get; set; }
        public ISet<MigrationMetaInformation> MetaMigrations { get; set; }
    }
}
