using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.IO;
using System.Threading;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public abstract class DatabaseTestFrameworkTemplate : IDisposable
    {
        public DbConnection Connection { get; private set; }
        public string ConnectionString { get; private set; }
        public bool IsConnected { get; private set; }
        private readonly string _DockerComposeArgumentPrefix;
        private bool _Disposed = false;
        private readonly string _TestDatabaseFolder;
        private readonly IGenericDatabaseInteractor _GenericDatabaseInteractor;
        public abstract string GetDatabaseName();
        public abstract DbConnection CreateConnection(string connectionString);
        public abstract void ConfigureDb<TDbContext>(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<TDbContext> optionsBuilder)
            where TDbContext : DbContext;

        public DatabaseTestFrameworkTemplate(string dockerProjectName, string connectionString, string testDatabaseFolder)
        {
            this.IsConnected = false;
            this._TestDatabaseFolder = testDatabaseFolder;
            this._DockerComposeArgumentPrefix = $"compose --project-name {dockerProjectName}";
            string argument = $"{this._DockerComposeArgumentPrefix} up --force-recreate --detach";
            GUtilities.EnsureDirectoryDoesNotExist(Path.Combine(this._TestDatabaseFolder, "Volumes"));
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", argument, this._TestDatabaseFolder);
            {
                externalProgramExecutor.Run();
                Thread.Sleep(TimeSpan.FromSeconds(3));//TODO replace this by wait until healthcheck says service is ready/healthy (with a timeout of 1 minute)
                GUtilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while starting test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
            }
            this.ConnectionString = connectionString;
            this.Connection = this.CreateConnection(this.ConnectionString);
            this._GenericDatabaseInteractor = DBUtilities.GetDatabaseInteractor(this.Connection);
            Tools.ConnectToDatabaseWrapper(() =>
              {
                  if (!externalProgramExecutor.IsRunning && externalProgramExecutor.ExitCode != 0)
                  {
                      throw new AbortException(new InternalAlgorithmException($"docker exited with exitcode {externalProgramExecutor.ExitCode}; StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErrt: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)};"));
                  }
                  this.Connection.Open();

                  using (DbDataReader reader = this._GenericDatabaseInteractor.CreateCommand("select 1;", this.Connection).ExecuteReader())
                  {
                      GUtilities.AssertCondition(reader.HasRows, "Test-statement did not return any row. So database-connection is not ready.");
                      while (reader.Read())
                      {
                          GUtilities.NoOperation(); // Just to ensure that we can read from the reader without any exceptions
                      }
                  }

                  this.IsConnected = true;
              }, GeneralLogger.NoLog(), this._GenericDatabaseInteractor.AdaptConnectionString(this.Connection.ConnectionString));
        }

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
                            this.Connection.Close();
                        }
                        this.Connection.Dispose();
                    }
                    using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", $"{this._DockerComposeArgumentPrefix} down", this._TestDatabaseFolder);
                    externalProgramExecutor.Run();
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    GUtilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while stopping test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
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
