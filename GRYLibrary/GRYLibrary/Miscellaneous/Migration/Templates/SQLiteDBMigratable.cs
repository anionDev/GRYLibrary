using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;

namespace GRYLibrary.Core.Miscellaneous.Migration.Templates
{
    public class SQLiteDBMigratable : Migratable
    {
        private readonly Version3 _CurrentVersionOfProgram;
        public SQLiteDBMigratable(IGeneralLogger logger, MigrationConfiguration migrationConfiguration, Version3 currentVersionOfProgram) : base(logger, migrationConfiguration)
        {
            this._CurrentVersionOfProgram = currentVersionOfProgram;
        }

        public override Version3 GetCurrentVersionOfExistingData()
        {
            throw new NotImplementedException();
        }

        public override Version3 GetCurrentVersionOfProgram()
        {
            throw new NotImplementedException();
        }

        public override bool InitializeWithLatestVersion()
        {
            throw new NotImplementedException();
        }

        public override bool IsAlreadyInitialized()
        {
            throw new NotImplementedException();
        }
    }
}
