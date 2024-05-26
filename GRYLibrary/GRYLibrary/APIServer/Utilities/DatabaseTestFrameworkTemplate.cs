using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using MySqlConnector;
using System;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public class DatabaseTestFrameworkTemplate : IDisposable
    {
        public MySqlConnection MySqlConnection { get; private set; }
        public string ConnectionString { get; private set; }
        public bool IsConnected { get; private set; }
        private readonly string _DockerComposeArgumentPrefix;
        private bool _Disposed = false;
        private readonly string _TestDatabaseFolder;

        public DatabaseTestFrameworkTemplate(string dockerProjectName, string connectionString, string testDatabaseFolder)
        {
            this.IsConnected = false;
            this._TestDatabaseFolder = testDatabaseFolder;
            this._DockerComposeArgumentPrefix = $"compose -p {dockerProjectName}";
            this.ConnectionString = connectionString;
            using (ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", $"{this._DockerComposeArgumentPrefix} up -d", this._TestDatabaseFolder))
            {
                externalProgramExecutor.Run();
            }
            Tools.ConnectToDatabase(() =>
            {
                this.MySqlConnection = new MySqlConnection(this.ConnectionString);
                this.MySqlConnection.Open();
                this.IsConnected = true;
            }, GeneralLogger.NoLog(), GUtilities.AdaptMariaDBSQLConnectionString(this.ConnectionString, true));
        }
        public void Dispose()
        {
            if (!this._Disposed)
            {
                if (this.MySqlConnection != null)
                {
                    if (this.IsConnected)
                    {
                        this.MySqlConnection.Dispose();
                    }
                }
                using (ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", $"{this._DockerComposeArgumentPrefix} down", this._TestDatabaseFolder))
                {
                    externalProgramExecutor.Run();
                }
                this._Disposed = true;
            }
        }
    }
}
