using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases;
using System;

namespace GRYLibrary.Core.APIServer.Services.Database.Visitors
{
    public class ToDatabaseInteractor : ISupportedDatabaseVisitor<IGenericDatabaseInteractor>
    {
        public IGenericDatabaseInteractor Handle(MariaDB mariaDB)
        {
            return new MariaDBDatabaseInteractor();
        }

        public IGenericDatabaseInteractor Handle(PostgreSQL postgreSQL)
        {
            return new PostgreSQLDatabaseInteractor();
        }

        public IGenericDatabaseInteractor Handle(SQLServer sQLServer)
        {
            throw new NotSupportedException();
        }

        public IGenericDatabaseInteractor Handle(Oracle oracle)
        {
            throw new NotSupportedException();
        }
    }
}
