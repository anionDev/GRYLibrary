namespace GRYLibrary.Core.APIServer.Services.Database
{
    public interface IDatabasePersistenceConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string DatabaseType { get; set; }
    }
}
