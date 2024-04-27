using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Services.Database
{
   public interface IGenericDatabaseInteractor
    {
        public DbCommand CreateCommand(string sql, DbConnection connection);
    }
}
