using System.Collections.Generic;
using System.Data.Common;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public interface IGenericDatabaseInteractor
    {
        public DbCommand CreateCommand(string sql, DbConnection connection);
        public IList<string> GetAllTableNames(DbConnection connection);
    }
}
