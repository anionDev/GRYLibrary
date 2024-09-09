using MySqlConnector;
using System.Collections.Generic;
using System.Data.Common;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public class MariaDBDatabaseInteractor : IGenericDatabaseInteractor
    {
        public DbCommand CreateCommand(string sql, DbConnection connection) => new MySqlCommand(sql, (MySqlConnection)connection);

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
    }
}
