using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Data.Common;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public static class DBUtilities
    {
        public static GenericDatabaseInteractor ToGenericDatabaseInteractor(IDatabasePersistenceConfiguration databasePersistenceConfiguration, IGRYLog log)
        {
            switch (databasePersistenceConfiguration.DatabaseType)
            {
                case "MariaDB":
                    return new MariaDBDatabaseInteractor(databasePersistenceConfiguration, log);
                case "PostgreSQL":
                    return new PostgreSQLDatabaseInteractor(databasePersistenceConfiguration, log);
                case "Oracle":
                    return new OracleDatabaseInteractor(databasePersistenceConfiguration, log);
                case "SQLServer":
                    return new SQLServerDatabaseInteractor(databasePersistenceConfiguration, log);
                default:
                    throw new NotSupportedException($"Database type {databasePersistenceConfiguration.DatabaseType} is not supported.");
            }
        }
        public static T? GetNullableValue<T>(DbDataReader reader, int parameterIndex)
        {
            if (reader.IsDBNull(parameterIndex))
            {
                return default(T);
            }
            else
            {
                return (T)reader.GetValue(parameterIndex);
            }
        }
    }
}
