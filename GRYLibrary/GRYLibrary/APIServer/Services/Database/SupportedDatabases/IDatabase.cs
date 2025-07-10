namespace GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases
{
    public interface IDatabase
    {
        public abstract void Accept(ISupportedDatabaseVisitor visitor);
        public abstract T Accept<T>(ISupportedDatabaseVisitor<T> visitor);
    }
    public interface ISupportedDatabaseVisitor
    {
        void Handle(MariaDB mariaDB);
        void Handle(PostgreSQL postgreSQL);
        void Handle(SQLServer sQLServer);
        void Handle(Oracle oracle);
    }
    public interface ISupportedDatabaseVisitor<T>
    {
        T Handle(MariaDB mariaDB);
        T Handle(PostgreSQL postgreSQL);
        T Handle(SQLServer sQLServer);
        T Handle(Oracle oracle);
    }
}
