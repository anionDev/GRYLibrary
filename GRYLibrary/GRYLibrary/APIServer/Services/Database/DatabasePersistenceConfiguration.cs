namespace GRYLibrary.Core.APIServer.Services.Database
{
    public class DatabasePersistenceConfiguration : IDatabasePersistenceConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string DatabaseType { get; set; }
    }
}
