using GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases;
using MySqlConnector;
using System;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator
{
    public class MariaDBDatabaseInteractor : IGenericDatabaseInteractor
    {
        public IDatabase ToSupportedDatabase()
        {
            return new MariaDB();
        }
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        private static readonly Regex PasswordHideRegex = new Regex("(PWD|Pwd)=([^;]+)(;|$)");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        public string AdaptConnectionString(string connectionString)
        {
            string replaceString = "********";
            connectionString = PasswordHideRegex.Replace(connectionString, match => $"{match.Groups[1]}={replaceString}{match.Groups[3]}");
            return connectionString;
        }

        public DbCommand CreateCommand(string sql, DbConnection connection)
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(connection.State == System.Data.ConnectionState.Open, $"Connection of {connection.GetType().Name} is not open.");
            return new MySqlCommand(sql, (MySqlConnection)connection);
        }

        public string CreateSQLStatementForCreatingMigrationMaintenanceTableIfNotExist(string migrationTableName)
        {
            return @$"create table if not exists {migrationTableName}(MigrationName varchar(255), ExecutionTimestamp datetime);";
        }

        public string GetSQLStatementForRunningMigration(string migrationContent, string migrationTableName, string migrationName, DateTimeOffset now)
        {
            var noUtc = now.ToUniversalTime();
            return @$"SET autocommit=0;
{migrationContent}
insert into {migrationTableName}(MigrationName, ExecutionTimestamp) values ('{migrationName}', '{noUtc:yyyy-MM-dd HH:mm:ss}')
";
        }

        public string GetSQLStatementForSelectMigrationMaintenanceTableContent(string migrationTableName)
        {
            return $"select MigrationName, ExecutionTimestamp from {migrationTableName};";
        }

        public string CreateSQLStatementForGetAllTableNames()
        {
            return $"show tables;";
        }
    }
}
