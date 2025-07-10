using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;

namespace GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases
{
    public class PostgreSQL : IDatabase
    {
        public void Accept(ISupportedDatabaseVisitor visitor)
        {
            visitor.Handle(this);
        }

        public T Accept<T>(ISupportedDatabaseVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        public IGenericDatabaseInteractor ToDatabaseInteractor()
        {
            return new PostgreSQLDatabaseInteractor();
        }
    }
}
