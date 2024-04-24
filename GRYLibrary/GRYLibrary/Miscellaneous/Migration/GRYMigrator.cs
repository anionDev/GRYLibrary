using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    /// <remarks>
    /// Only MariaDB is supported yet.
    /// </remarks>
    public class GRYMigrator
    {
        private readonly IGeneralLogger _Logger;
        private readonly ITimeService _TimeService;
        private readonly MySqlConnection _Connection;//TODO make this workable also for other databases
        private readonly IList<MigrationInstance> _Migrations;
        public const string MigrationTableName = "GRYMigrationInformation";
        public GRYMigrator(IGeneralLogger logger, ITimeService timeService, MySqlConnection connection, IList<MigrationInstance> migrations)
        {
            this._Logger = logger;
            this._TimeService = timeService;
            this._Connection = connection;
            this._Migrations = migrations;
        }
        /// <remarks>
        /// If the migration fails it will be rolled back, but this rollback does not apply for DDL-operations (like create table for example) because transactional DDL operations are still an open issue in MariaDB. See https://jira.mariadb.org/browse/MDEV-4259 .
        /// </remarks>
        public void InitializeDatabaseAndMigrateIfRequired()
        {
            using (MySqlCommand cmd = new MySqlCommand($"create table if not exists {MigrationTableName}(MigrationName varchar(255), ExecutionTimestamp datetime);", this._Connection))
            {
                cmd.ExecuteNonQuery();
            }

            IList<string> namesOfAlreadyExecutedMigrations = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand($"select MigrationName from {MigrationTableName};", this._Connection))
            {
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    namesOfAlreadyExecutedMigrations.Add(reader.GetString(0));
                }
            }

            foreach (MigrationInstance migration in this._Migrations)
            {
                if (!namesOfAlreadyExecutedMigrations.Contains(migration.MigrationName))
                {
                    this._Logger.Log($"Run Migration {migration.MigrationName}.", Microsoft.Extensions.Logging.LogLevel.Information);
                    DateTime now = _TimeService.GetCurrentTime();
                    string sql = "SET autocommit=0;" + Environment.NewLine + migration.MigrationContent + Environment.NewLine + $"insert into {MigrationTableName}(MigrationName, ExecutionTimestamp) values ('{migration.MigrationName}', '{now:yyyy-MM-dd HH:mm:ss}')";
                    Exception exception = null;
                    using (MySqlCommand sqlCommand = new MySqlCommand(sql, this._Connection))
                    {
                        using MySqlTransaction transaction = _Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        sqlCommand.Connection = _Connection;
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
            }
            this._Logger.Log("Finished database migration", Microsoft.Extensions.Logging.LogLevel.Information);
        }
        public static IList<MigrationInstance> LoadMigrationsFromResources(string migrationsResourceNamePrefix)
        {
            IList<MigrationInstance> migrationInstances = new List<MigrationInstance>();
            var assembly = Assembly.GetEntryAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.StartsWith(migrationsResourceNamePrefix))
                {
                    using Stream stream = assembly.GetManifestResourceStream(resourceName);
                    GUtilities.AssertCondition(stream != null, $"Migration-resource '{resourceName}' could not be loaded.");
                    using StreamReader reader = new StreamReader(stream);
                    string migrationName = resourceName[migrationsResourceNamePrefix.Length..^4];
                    string resourceContent = reader.ReadToEnd();
                    migrationInstances.Add(new MigrationInstance
                    {
                        MigrationName = migrationName,
                        MigrationContent = resourceContent,
                    });
                }
            }
            migrationInstances = migrationInstances.OrderBy(migration => migration.MigrationName).ToList();
            return migrationInstances;
        }
    }
}
