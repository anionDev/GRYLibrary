using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GRYLibrary.Core.Miscellaneous.Migration
{
    public class GRYMigrator
    {
        private readonly IGeneralLogger _Logger;
        private readonly ITimeService _TimeService;
        private readonly SqlConnection _Connection;
        private readonly IList<MigrationInstance> _Migrations;
        public GRYMigrator(IGeneralLogger logger, ITimeService timeService, SqlConnection connection, IList<MigrationInstance> migrations)
        {
            this._Logger = logger;
            this._TimeService = timeService;
            this._Connection = connection;
            this._Migrations = migrations;
        }
        public void InitializeDatabaseAndMigrateIfRequired()
        {
            using (SqlCommand cmd = new SqlCommand("create table if not exists GRYMigrationInformation([MigrationName] varchar(255), [ExecutionTimestamp] datetime);", this._Connection))
            {
                cmd.ExecuteNonQuery();
            }

            IList<string> namesOfAlreadyExecutedMigrations = new List<string>();
            using (SqlCommand cmd = new SqlCommand("select [MigrationName] from GRYMigrationInformation;", this._Connection))
            {
                SqlDataReader reader = cmd.ExecuteReader();
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
                    using (SqlCommand cmd = new SqlCommand(migration.MigrationContent, this._Connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand($"insert into GRYMigrationInformation([MigrationName], [ExecutionTimestamp]) values ('{migration.MigrationName}', '{now:yyyy-MM-dd HH:mm:ss}')", this._Connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            this._Logger.Log("Finished database migration", Microsoft.Extensions.Logging.LogLevel.Information);
        }

    }
}
