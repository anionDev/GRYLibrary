using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using MySqlConnector;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

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
            this._DockerComposeArgumentPrefix = $"compose --project-name {dockerProjectName}";
            this.ConnectionString = connectionString;
            string argument = $"{this._DockerComposeArgumentPrefix} up --force-recreate --detach";
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", argument, this._TestDatabaseFolder);
            {
                externalProgramExecutor.Run();
            }
            Tools.ConnectToDatabaseWrapper(() =>
              {
                  if (!externalProgramExecutor.IsRunning && externalProgramExecutor.ExitCode!=0)
                  {
                      throw new AbortException(new InternalAlgorithmException($"docker exited with exitcode {externalProgramExecutor.ExitCode}; StdOut: {Environment.NewLine + string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErrt: {Environment.NewLine + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)};"));
                  }
                  this.MySqlConnection = new MySqlConnection(this.ConnectionString);
                  this.MySqlConnection.Open();
                  this.IsConnected = true;
              }, GeneralLogger.NoLog(), GUtilities.AdaptMariaDBSQLConnectionString(this.ConnectionString, true));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    if (this.MySqlConnection != null)
                    {
                        if (this.IsConnected)
                        {
                            this.MySqlConnection.Dispose();
                        }
                    }
                    using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", $"{this._DockerComposeArgumentPrefix} down", this._TestDatabaseFolder);
                    externalProgramExecutor.Run();
                }
                this._Disposed = true;
            }
        }

        public void Dispose() // Implement IDisposable
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DatabaseTestFrameworkTemplate() // the finalizer
        {
            this.Dispose(false);
        }
    }
}
