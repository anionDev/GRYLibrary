using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public abstract class DatabaseTestFrameworkTemplate : IDisposable
    {
        public DbConnection Connection { get; private set; }
        public string ConnectionString { get; private set; }
        public bool IsConnected { get; private set; }
        private bool _Disposed = false;
        private readonly string _TestDatabaseFolder;
        private readonly IGenericDatabaseInteractor _GenericDatabaseInteractor;
        public abstract string GetDatabaseTypeName();
        private readonly string _TaskNameStart;
        private readonly string _TaskNameStop;
        private readonly IGRYLog _Log;
        private readonly string _ResetDatabaseScript;
        public DatabaseTestFrameworkTemplate(IDatabasePersistenceConfiguration configuration, string testDatabaseFolder, string repositoryFolder, string taskNameStart, string taskNameStop, string resetDatabaseScript, IGRYLog log, TimeSpan connectionTimeout, ISet<string> containerNamesToWaitToBeHealthy)
        {
            this._TaskNameStart = taskNameStart;
            this._TaskNameStop = taskNameStop;
            this._ResetDatabaseScript = resetDatabaseScript;
            this._Log = log;
            try
            {
                this.IsConnected = false;
                this._TestDatabaseFolder = testDatabaseFolder;
                string argument = this._TaskNameStart;
                string volumesFolder = Path.Combine(this._TestDatabaseFolder, "Volumes", "Data");
                using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("task", argument, repositoryFolder);
                {
                    externalProgramExecutor.Run();
                    GUtilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while starting test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
                    GUtilities.AssertCondition(GUtilities.RunWithTimeout(() =>
                    {
                        foreach (var containerNameToWaitToBeHealthy in containerNamesToWaitToBeHealthy)
                        {
                            GUtilities.AssertCondition(GUtilities.ContainerIsHealthy(containerNameToWaitToBeHealthy), $"Container {containerNameToWaitToBeHealthy} did not become healthy within the expected time.");
                        }
                    }, connectionTimeout));
                }
                this._GenericDatabaseInteractor = DBUtilities.ToGenericDatabaseInteractor(configuration, log);
                Exception? lastException = null;
                if (!GUtilities.RunWithTimeout(() =>
                {
                    bool connected = false;
                    while (!connected)
                    {
                        (bool, Exception?) isAvailable = this._GenericDatabaseInteractor.IsAvailable();
                        connected = isAvailable.Item1;
                        lastException = isAvailable.Item2;
                        if (!connected)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                    }
                }, connectionTimeout))
                {
                    string message = $"{this.GetType().Name} was not able to connect to the test-database.";
                    if (lastException == null)
                    {
                        throw new NotReadyException(message);
                    }
                    else
                    {
                        throw new NotReadyException(message, lastException);
                    }
                }
            }
            catch
            {
                throw;
            }
        }


        public IGenericDatabaseInteractor GenericDatabaseInteractor()
        {
            return this._GenericDatabaseInteractor;
        }
        protected virtual void Dispose(bool disposing)
        {
            try
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
                        using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("task", this._TaskNameStop, this._TestDatabaseFolder);
                        externalProgramExecutor.Configuration.WaitingState = new RunSynchronously();
                        externalProgramExecutor.Run();
                        GUtilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while stopping test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
                    }
                    this._GenericDatabaseInteractor.Dispose();
                    this._Disposed = true;
                }
            }
            catch
            {
                throw;
            }
        }

        public void ResetDatabase()
        {
            if (0 < this.GenericDatabaseInteractor().GetAllTableNames().ToList().Count)
            {
                DbConnection connection = this._GenericDatabaseInteractor.GetConnection();
                try
                {
                    using DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = this._ResetDatabaseScript;
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
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
