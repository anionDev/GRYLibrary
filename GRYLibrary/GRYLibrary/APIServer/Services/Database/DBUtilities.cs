using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using GRYLibrary.Core.Misc;
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
        public static T? GetValue<T>(DbDataReader reader, int parameterIndex, bool allowNull)
        {
            if (allowNull && reader.IsDBNull(parameterIndex))
            {
                return default(T);
            }
            return ConvertValue<T>(reader.GetValue(parameterIndex));
        }

        public static T? ConvertValue<T>(object value)
        {
            if (typeof(T).Equals(typeof(GRYDateTime)))
            {
                DateTime extractedValue = ConvertValue<DateTime>(value);
                return (T)(object)GRYDateTime.FromDateTime(extractedValue);
            }
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                return (T)value;
            }
        }
    }
}
