using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        private IGenericDatabaseInteractor _GenericDatabaseInteractor;
        public abstract string GetDatabaseName();
        public abstract DbConnection CreateConnection(string connectionString);
        public abstract void ConfigureDb<TDbContext>(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<TDbContext> optionsBuilder)
            where TDbContext : DbContext;
        private readonly string _TaskNameStart;
        private readonly string _TaskNameStop;
        public DatabaseTestFrameworkTemplate(string dockerProjectName, string connectionString, string testDatabaseFolder, string repositoryFolder, string taskNameStart, string taskNameStop, string resetDatabaseScript)
        {
            _TaskNameStart = taskNameStart;
            _TaskNameStop = taskNameStop;
            try
            {
                this.IsConnected = false;
                this._TestDatabaseFolder = testDatabaseFolder;
                this._DockerComposeArgumentPrefix = $"compose --project-name {dockerProjectName}";
                string argument = _TaskNameStart;
                string volumesFolder = Path.Combine(this._TestDatabaseFolder, "Volumes", "Data");
                using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("task", argument, repositoryFolder);
                {
                    externalProgramExecutor.Configuration.WaitingState = new RunSynchronously();
                    externalProgramExecutor.Run();
                    Thread.Sleep(TimeSpan.FromSeconds(5));//TODO replace this by wait until healthcheck says service is ready/healthy (with a timeout of 1 minute)
                    GUtilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while starting test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
                }
                Tools.ConnectToDatabaseWrapper(() => TryToConnect(externalProgramExecutor, resetDatabaseScript, connectionString), GeneralLogger.NoLog(), connectionString);//TODO adapt connectionstring
            }
            catch
            {
                throw;
            }
        }
        private void TryToConnect(ExternalProgramExecutor externalProgramExecutor, string resetDatabaseScript, string connectionString)
        {

            DbConnection connection = this.CreateConnection(connectionString);
            connection.Open();
            try
            {

                this._GenericDatabaseInteractor = DBUtilities.GetDatabaseInteractor(connection);
                if (!externalProgramExecutor.IsRunning && externalProgramExecutor.ExitCode != 0)
                {
                    throw new AbortException(new InternalAlgorithmException($"docker exited with exitcode {externalProgramExecutor.ExitCode}; StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErrt: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)};"));
                }


                using (DbDataReader reader = this._GenericDatabaseInteractor.CreateCommand("select 1;", connection).ExecuteReader())
                {
                    GUtilities.AssertCondition(reader.HasRows, "Test-statement did not return any row. So database-connection is not ready.");
                    while (reader.Read())
                    {
                        GUtilities.NoOperation(); // Just to ensure that we can read from the reader without any exceptions
                    }
                }

            }
            catch
            {
                connection.Dispose();
                throw;
            }

            this.IsConnected = true;
            Connection = connection;

                 List<string> tables = new List<string>();// List<string> tables = GRYMigrator.GetAllTableNames(Connection).ToList();
                if (1 < tables.Count)
                {
                    using var tx = Connection.BeginTransaction();
                    try
                    {
                        using var cmd = Connection.CreateCommand();
                        cmd.Transaction = tx;
                        cmd.CommandText = resetDatabaseScript;
                        cmd.ExecuteNonQuery();
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                      //  throw; //TODO this goes wrong if the database does not have tables yet. therefore this must be checked before and then this try-catch-block can be removed.
           
                    }
                }

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
                    using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("task", _TaskNameStop, this._TestDatabaseFolder);
                    externalProgramExecutor.Configuration.WaitingState = new RunSynchronously();
                    externalProgramExecutor.Run();
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
