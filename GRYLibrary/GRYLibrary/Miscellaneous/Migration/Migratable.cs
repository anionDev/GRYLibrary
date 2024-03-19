using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public abstract class Migratable
    {
        public MigrationConfiguration MigrationConfiguration { get; }
        private readonly IGeneralLogger _Logger;
        public Migratable(IGeneralLogger logger, MigrationConfiguration migrationConfiguration)
        {
            this.MigrationConfiguration = migrationConfiguration;
            this._Logger = logger;
        }
        public abstract bool IsAlreadyInitialized();
        public abstract bool InitializeWithLatestVersion();
        public abstract Version3 GetCurrentVersionOfExistingData();
        public abstract Version3 GetCurrentVersionOfProgram();
        public void InitializeDatabaseAndMigrateIfRequired()
        {
            if (this.IsAlreadyInitialized())
            {
                var migrations = this.GetSortedRelevantMigrations();
                this.ValidateMigrations(migrations);
                foreach (var migration in migrations)
                {
                    migration.Migration(migration.MigratationInformation);
                }
            }
            else
            {
                this.InitializeWithLatestVersion();
            }
        }

        private void ValidateMigrations(IEnumerable<MigrationMetaInformation> migrations)
        {
            foreach (var migration in migrations)
            {
                this.ValidateMigration(migration);
            }
        }

        private void ValidateMigration(MigrationMetaInformation migration)
        {
            if (!(migration.MigratationInformation.SourceVersion < migration.MigratationInformation.TargetVersion))
            {
                throw new System.IO.InvalidDataException($"Can not migrate from verstion v{migration.MigratationInformation.SourceVersion} to v{migration.MigratationInformation.TargetVersion}.");
            }
            //add more checks if desired
        }

        private IList<MigrationMetaInformation> GetSortedRelevantMigrations()
        {
            return this.MigrationConfiguration.MigrationMetaInformations
                .Where(migrationMetaInformation =>
                    this.GetCurrentVersionOfExistingData() <= migrationMetaInformation.MigratationInformation.SourceVersion
                    && migrationMetaInformation.MigratationInformation.TargetVersion <= this.GetCurrentVersionOfProgram())
                .OrderBy(m => m.MigratationInformation.SourceVersion)
                .ToList();
        }
    }
}
