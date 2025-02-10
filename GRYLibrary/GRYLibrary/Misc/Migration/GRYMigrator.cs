using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.Misc.Migration
{
    /// <remarks>
    /// Only MariaDB is supported yet.
    /// </remarks>
    public class GRYMigrator
    {
        private readonly IGeneralLogger _Logger;
        private readonly ITimeService _TimeService;
        private readonly DbConnection _Connection;
        private readonly IList<MigrationInstance> _Migrations;
        public const string MigrationTableName = "GRYMigrationInformation";
        private readonly IGenericDatabaseInteractor _DatabaseInteractor;
        public GRYMigrator(IGeneralLogger logger, ITimeService timeService, DbConnection connection, IList<MigrationInstance> migrations, IGenericDatabaseInteractor databaseInteractor)
        {
            this._Logger = logger;
            this._TimeService = timeService;
            this._Connection = connection;
            this._Migrations = migrations;
            this._DatabaseInteractor = databaseInteractor;
        }
        /// <remarks>
        /// If the migration fails it will be rolled back, but this rollback does not apply for DDL-operations (like create table for example) because transactional DDL operations are still an open issue in MariaDB. See https://jira.mariadb.org/browse/MDEV-4259 .
        /// </remarks>
        public void InitializeDatabaseAndMigrateIfRequired()
        {
            using (DbCommand cmd = this._DatabaseInteractor.CreateCommand($"create table if not exists {MigrationTableName}(MigrationName varchar(255), ExecutionTimestamp datetime);", this._Connection))
            {
                cmd.ExecuteNonQuery();
            }

            IEnumerable<string> namesOfAlreadyExecutedMigrations = this.GetExecutedMigrations().Select(m => m.MigrationName);

            IList<MigrationInstance> migrationsToRun = new List<MigrationInstance>();
            foreach (MigrationInstance migration in this._Migrations)
            {
                if (!namesOfAlreadyExecutedMigrations.Contains(migration.MigrationName))
                {
                    migrationsToRun.Add(migration);
                }
            }
            if (migrationsToRun.Count == 0)
            {
                this._Logger.Log("No database-migrations found which were not already executed.", Microsoft.Extensions.Logging.LogLevel.Information);
            }
            else
            {
                this._Logger.Log($"{migrationsToRun.Count} database-migration(s) to run found.", Microsoft.Extensions.Logging.LogLevel.Information);
                foreach (MigrationInstance migration in migrationsToRun)
                {
                    this._Logger.Log($"Run Migration {migration.MigrationName}.", Microsoft.Extensions.Logging.LogLevel.Information);
                    DateTime now = this._TimeService.GetCurrentTime();
                    string sql = "SET autocommit=0;" + Environment.NewLine + migration.MigrationContent + Environment.NewLine + $"insert into {MigrationTableName}(MigrationName, ExecutionTimestamp) values ('{migration.MigrationName}', '{now:yyyy-MM-dd HH:mm:ss}')";
                    Exception exception = null;
                    using (DbCommand sqlCommand = this._DatabaseInteractor.CreateCommand(sql, this._Connection))
                    {
                        using DbTransaction transaction = this._Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        sqlCommand.Connection = this._Connection;
                        sqlCommand.Transaction = transaction;
                        try
                        {
                            sqlCommand.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();//HINT problem in mariadb here: you can not revert something like a create-table-statement (see https://stackoverflow.com/a/4736346/3905529 )
                            this._Logger.Log($"Error in Migration {migration.MigrationName}.", Microsoft.Extensions.Logging.LogLevel.Error);
                            exception = e;
                        }
                    }
                    if (exception != null)
                    {
                        throw exception;
                    }
                }
                this._Logger.Log("Finished database migration", Microsoft.Extensions.Logging.LogLevel.Information);
            }
        }
        public static IList<MigrationInstance> LoadMigrationsFromResources(Assembly assembly, string migrationsResourceNamePrefix)
        {
            IList<MigrationInstance> migrationInstances = new List<MigrationInstance>();
            uint i = 0;
            List<string> resources = assembly.GetManifestResourceNames().Order().ToList();
            foreach (string resourceName in resources)
            {
                if (resourceName.StartsWith(migrationsResourceNamePrefix))
                {
                    using Stream stream = assembly.GetManifestResourceStream(resourceName);
                    GUtilities.AssertCondition(stream != null, $"Migration-resource '{resourceName}' could not be loaded.");
                    using StreamReader reader = new StreamReader(stream);
                    string migrationName = resourceName[migrationsResourceNamePrefix.Length..^4];
                    string resourceContent = reader.ReadToEnd();
                    migrationInstances.Add(new MigrationInstance(i, migrationName, resourceContent));
                    i = i + 1;
                }
            }
            migrationInstances = migrationInstances.OrderBy(migration => migration.Index).ToList();
            return migrationInstances;
        }
        public IList<MigrationExecutionInformation> GetExecutedMigrations()
        {
            IList<MigrationExecutionInformation> result = new List<MigrationExecutionInformation>();
            using (DbCommand cmd = this._DatabaseInteractor.CreateCommand($"select MigrationName, ExecutionTimestamp from {MigrationTableName};", this._Connection))
            {
                using DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new MigrationExecutionInformation(reader.GetString(0), reader.GetDateTime(1)));
                }
            }
            return result.OrderBy(o => o.ExecutionTimestamp).ToList();
        }
        /// <remarks>
        /// This function is supposed to be a utilitiy for an integration-test.
        /// </remarks>
        public static void DoAllMigrations(DbConnection dbConnection, IDatabaseManager databaseManager,ITimeService timeService)
        {
            IList<MigrationInstance> migrations = databaseManager.GetAllMigrations();
            GRYMigrator migrator = new GRYMigrator(GeneralLogger.CreateUsingConsole(), timeService, dbConnection, migrations, databaseManager.GetGenericDatabaseInteractor());
            migrator.InitializeDatabaseAndMigrateIfRequired();
            GUtilities.AssertCondition(databaseManager.GetGenericDatabaseInteractor().GetAllTableNames(dbConnection).Any());
        }
    }
}