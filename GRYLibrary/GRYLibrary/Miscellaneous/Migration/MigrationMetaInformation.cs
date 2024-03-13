using System;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class MigrationMetaInformation
    {
        public MigratationInformation MigratationInformation { get; set; }
        public Action<MigratationInformation> Migration { get; set; }
    }
}
