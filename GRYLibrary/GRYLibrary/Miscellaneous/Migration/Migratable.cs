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
                IList<MigrationMetaInformation> migrations = this.GetSortedRelevantMigrations();
                this.ValidateMigrations(migrations);
                foreach (MigrationMetaInformation migration in migrations)
                {
                    this._Logger.Log($"Migrate database from v{migration.MigratationInformation.SourceVersion} to v{migration.MigratationInformation.TargetVersion}.", Microsoft.Extensions.Logging.LogLevel.Information);
                    migration.Migration(migration.MigratationInformation);
                }
            }
            else
            {
                this._Logger.Log("Initialize database", Microsoft.Extensions.Logging.LogLevel.Information);
                this.InitializeWithLatestVersion();
            }
            this._Logger.Log("Finished database initialization", Microsoft.Extensions.Logging.LogLevel.Information);
        }

        private void ValidateMigrations(IEnumerable<MigrationMetaInformation> migrations)
        {
            foreach (MigrationMetaInformation migration in migrations)
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
