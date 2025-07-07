using MySqlConnector;
using Npgsql;
using System;
using System.Data.Common;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public class DBUtilities
    {
        public static IGenericDatabaseInteractor GetDatabaseInteractor(DbConnection dbConnection)
        {
            if (dbConnection is MySqlConnection)
            {
                return new MariaDBDatabaseInteractor();
            }
            else if (dbConnection is NpgsqlConnection)
            {
                return new PostgreSQLDatabaseInteractor();
            }
            else
            {
                throw new NotSupportedException($"The database type {dbConnection.GetType().Name} is not supported yet.");
            }
        }
    }
}
