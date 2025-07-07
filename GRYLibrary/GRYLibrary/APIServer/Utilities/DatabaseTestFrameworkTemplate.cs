using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Data.Common;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public abstract class DatabaseTestFrameworkTemplate : IDisposable
    {
        public System.Data.Common.DbConnection Connection { get; private set; }
        public string ConnectionString { get { return Connection.ConnectionString; } }
        public bool IsConnected { get; private set; }
        private readonly string _DockerComposeArgumentPrefix;
        private bool _Disposed = false;
        private readonly string _TestDatabaseFolder;
        private readonly IGenericDatabaseInteractor _GenericDatabaseInteractor;

        public abstract string GetDatabaseName();
        public DatabaseTestFrameworkTemplate(string dockerProjectName, string connectionString, string testDatabaseFolder)
        {
            this.IsConnected = false;
            this._TestDatabaseFolder = testDatabaseFolder;
            this._DockerComposeArgumentPrefix = $"compose --project-name {dockerProjectName}";
            string argument = $"{this._DockerComposeArgumentPrefix} up --force-recreate --detach";
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", argument, this._TestDatabaseFolder);
            {
                externalProgramExecutor.Run();
            }
            this.Connection = CreateConnection(connectionString);
            this._GenericDatabaseInteractor = DBUtilities.GetDatabaseInteractor(Connection);
            Tools.ConnectToDatabaseWrapper(() =>
              {
                  if (!externalProgramExecutor.IsRunning && externalProgramExecutor.ExitCode != 0)
                  {
                      throw new AbortException(new InternalAlgorithmException($"docker exited with exitcode {externalProgramExecutor.ExitCode}; StdOut: {Environment.NewLine + string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErrt: {Environment.NewLine + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)};"));
                  }
                  this.Connection.Open();
                  this.IsConnected = true;
              }, GeneralLogger.NoLog(), this._GenericDatabaseInteractor.AdaptConnectionString(this.Connection.ConnectionString));
            //TODO reset database
        }

        public abstract DbConnection CreateConnection(string connectionString);

        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    if (this.Connection != null)
                    {
                        if (this.IsConnected)
                        {
                            this.Connection.Dispose();
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
