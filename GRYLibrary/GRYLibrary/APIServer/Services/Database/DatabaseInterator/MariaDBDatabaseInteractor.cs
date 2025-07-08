using GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases;
using MySqlConnector;
using System;
using System.Collections.Generic;
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
        private static readonly Regex PasswordHideRegex = new Regex("(PWD|Pwd)=([^;]+)(;|$)");
        public string AdaptConnectionString(string connectionString)
        {
            string replaceString = "********";
            connectionString = PasswordHideRegex.Replace(connectionString, match => $"{match.Groups[1]}={replaceString}{match.Groups[3]}");
            return connectionString;
        }

        public DbCommand CreateCommand(string sql, DbConnection connection)
        {
            return new MySqlCommand(sql, (MySqlConnection)connection);
        }

        public string CreateSQLStatementForCreatingMigrationMaintenanceTableIfNotExist(string migrationTableName)
        {
            return @$"create table if not exists {migrationTableName}(MigrationName varchar(255), ExecutionTimestamp datetime);";
        }

        public IList<string> GetAllTableNames(DbConnection connection)
        {
            IList<string> result = new List<string>();
            using (DbCommand cmd = this.CreateCommand($"show tables;", connection))
            {
                using DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
            return result;
        }

        public string GetSQLStatementForRunningMigration(string migrationContent, string migrationTableName, string migrationName, DateTime now)
        {
            return @$"SET autocommit=0;
{migrationContent}
insert into {migrationTableName}(MigrationName, ExecutionTimestamp) values ('{migrationName}', '{now:yyyy-MM-dd HH:mm:ss}')
";
        }

        public string GetSQLStatementForSelectMigrationMaintenanceTableContent(string migrationTableName)
        {
            return $"select MigrationName, ExecutionTimestamp from {migrationTableName};";
        }
    }
}
