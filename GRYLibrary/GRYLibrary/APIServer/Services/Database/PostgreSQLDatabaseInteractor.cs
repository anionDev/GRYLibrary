using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public class PostgreSQLDatabaseInteractor : IGenericDatabaseInteractor
    {

        private static readonly Regex PasswordHideRegex = new Regex("(PWD|Pwd)=([^;]+)(;|$)");
        public DbCommand CreateCommand(string sql, DbConnection connection)
        {
              return new NpgsqlCommand(sql, (NpgsqlConnection)connection);
        }

        public IList<string> GetAllTableNames(DbConnection connection)
        {
            IList<string> result = new List<string>();
            using (DbCommand cmd = this.CreateCommand(@"SELECT tablename
FROM pg_catalog.pg_tables
WHERE schemaname NOT IN ('pg_catalog', 'information_schema');", connection))
            {
                using DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
            return result;
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

        public string GetSQLStatementForRunningMigration(string migrationContent, string migrationTableName, string migrationName, DateTime now)
        {
            return @$"
{migrationContent}
insert into {migrationTableName}(MigrationName, ExecutionTimestamp) values ('{migrationName}', '{now:yyyy-MM-dd HH:mm:ss}')
";
        }
    }
}
