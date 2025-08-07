using GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases;
using Npgsql;
using System;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator
{
    public class PostgreSQLDatabaseInteractor : IGenericDatabaseInteractor
    {
        public IDatabase ToSupportedDatabase()
        {
            return new PostgreSQL();
        }
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        private static readonly Regex PasswordHideRegex = new Regex("(PWD|Pwd)=([^;]+)(;|$)");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        public DbCommand CreateCommand(string sql, DbConnection connection)
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(connection.State == System.Data.ConnectionState.Open, $"Connection of {connection.GetType().Name} is not open.");
            return new NpgsqlCommand(sql, (NpgsqlConnection)connection);
        }

        public string AdaptConnectionString(string connectionString)
        {
            string replaceString = "********";
            connectionString = PasswordHideRegex.Replace(connectionString, match => $"{match.Groups[1]}={replaceString}{match.Groups[3]}");
            return connectionString;
        }

        public string CreateSQLStatementForCreatingMigrationMaintenanceTableIfNotExist(string migrationTableName)
        {
            return $@"CREATE TABLE IF NOT EXISTS {migrationTableName} (
    MigrationName VARCHAR(255),
    ExecutionTimestamp TIMESTAMP
);";
        }

        public string GetSQLStatementForSelectMigrationMaintenanceTableContent(string migrationTableName)
        {
            return $"select MigrationName, ExecutionTimestamp from {migrationTableName};";
        }

        public string GetSQLStatementForRunningMigration(string migrationContent, string migrationTableName, string migrationName, DateTimeOffset now)
        {
            var noUtc = now.ToUniversalTime();
            return @$"
{migrationContent}
insert into {migrationTableName}(MigrationName, ExecutionTimestamp) values ('{migrationName}', '{noUtc:yyyy-MM-dd HH:mm:ss}')
";
        }

        public string CreateSQLStatementForGetAllTableNames()
        {
            return @"SELECT tablename
FROM pg_catalog.pg_tables
WHERE schemaname NOT IN ('pg_catalog', 'information_schema');";
        }
    }
}
