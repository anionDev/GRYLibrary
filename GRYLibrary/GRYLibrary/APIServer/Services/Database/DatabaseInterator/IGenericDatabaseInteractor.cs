using GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases;
using System;
using System.Data.Common;

namespace GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator
{
    public interface IGenericDatabaseInteractor
    {
        public abstract IDatabase ToSupportedDatabase();
        public DbCommand CreateCommand(string sql, DbConnection connection);
        public string AdaptConnectionString(string connectionString);
        public string CreateSQLStatementForGetAllTableNames();
        public string CreateSQLStatementForCreatingMigrationMaintenanceTableIfNotExist(string tableName);
        public string GetSQLStatementForSelectMigrationMaintenanceTableContent(string migrationTableName);
        public string GetSQLStatementForRunningMigration(string migrationContent, string migrationTableName, string migrationName, DateTime now);
    }
}
