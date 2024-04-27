using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public class MariaDBDatabaseInteractor : IGenericDatabaseInteractor
    {
        public DbCommand CreateCommand(string sql, DbConnection connection)
        {
            return new MySqlCommand(sql, (MySqlConnection)connection);
        }
    }
}
