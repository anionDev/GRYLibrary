using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Npgsql;
using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public abstract class DatabaseTestFrameworkTemplate : IDisposable
    {
        public System.Data.Common.DbConnection Connection { get; private set; }
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
            GRYLibrary.Core.Misc.Utilities.EnsureDirectoryDoesNotExist(Path.Combine(_TestDatabaseFolder, "Volumes"));
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("docker", argument, this._TestDatabaseFolder);
            {
                externalProgramExecutor.Run();
                Thread.Sleep(TimeSpan.FromSeconds(3));//TODO replace this by wait until healthcheck says service is ready/healthy (with a timeout of 1 minute)
                GRYLibrary.Core.Misc.Utilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while starting test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
            }
            ConnectionString = connectionString;
            this.Connection = CreateConnection(ConnectionString);
            this._GenericDatabaseInteractor = DBUtilities.GetDatabaseInteractor(Connection);
            Tools.ConnectToDatabaseWrapper(() =>
              {
                  if (!externalProgramExecutor.IsRunning && externalProgramExecutor.ExitCode != 0)
                  {
                      throw new AbortException(new InternalAlgorithmException($"docker exited with exitcode {externalProgramExecutor.ExitCode}; StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErrt: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)};"));
                  }
                  this.Connection.Open();
                  //TODO do something like:
                  /*
                  using (DbCommand cmd = new NpgsqlCommand(@"select 1;", (NpgsqlConnection) Connection)) //test if connection works
                  {
                      using( DbDataReader reader = cmd.ExecuteReader()){ 
                      while (reader.Read())
                      {
                              //nothing to do
                      }
                      }
                  }
                  */
                  this.IsConnected = true;
              }, GeneralLogger.NoLog(), this._GenericDatabaseInteractor.AdaptConnectionString(this.Connection.ConnectionString));
            //TODO reset database
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
                    GRYLibrary.Core.Misc.Utilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while stopping test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
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
