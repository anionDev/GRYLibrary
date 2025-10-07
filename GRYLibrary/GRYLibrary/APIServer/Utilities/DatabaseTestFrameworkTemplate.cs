﻿using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
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
        private readonly string _DockerComposeArgumentPrefix;
        private bool _Disposed = false;
        private readonly string _TestDatabaseFolder;
        private IGenericDatabaseInteractor _GenericDatabaseInteractor;
        public abstract string GetDatabaseName();
        private readonly string _TaskNameStart;
        private readonly string _TaskNameStop;
        private readonly IGRYLog _Log;
        private readonly string _ResetDatabaseScript;
        public DatabaseTestFrameworkTemplate(string dockerProjectName, IDatabasePersistenceConfiguration configuration, string testDatabaseFolder, string repositoryFolder, string taskNameStart, string taskNameStop, string resetDatabaseScript, IGRYLog log)
        {
            this._TaskNameStart = taskNameStart;
            this._TaskNameStop = taskNameStop;
            this._ResetDatabaseScript = resetDatabaseScript;
            this._Log = log;
            try
            {
                this.IsConnected = false;
                this._TestDatabaseFolder = testDatabaseFolder;
                this._DockerComposeArgumentPrefix = $"compose --project-name {dockerProjectName}";
                string argument = this._TaskNameStart;
                string volumesFolder = Path.Combine(this._TestDatabaseFolder, "Volumes", "Data");
                using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("task", argument, repositoryFolder);
                {
                    externalProgramExecutor.Configuration.WaitingState = new RunSynchronously();
                    externalProgramExecutor.Run();
                    GUtilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while starting test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
                    Thread.Sleep(TimeSpan.FromSeconds(5));//TODO replace this by wait until healthcheck says service is ready/healthy (with a timeout of 1 minute)
                }
                this._GenericDatabaseInteractor = DBUtilities.ToGenericDatabaseInteractor(configuration, log);
                //Tools.ConnectToDatabaseWrapper(() => TryToConnect(externalProgramExecutor, resetDatabaseScript, configuration), GeneralLogger.NoLog());
                //TODO wait until database is ready
                GUtilities.RunWithTimeout(() =>
                {
                    bool connected = false;
                    while (!connected)
                    {
                        connected = this._GenericDatabaseInteractor.IsAvailable();
                        if (!connected)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                    }
                    int i = 3;
                }, Debugger.IsAttached ? TimeSpan.FromHours(2) : TimeSpan.FromSeconds(30));
                int i = 4;
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
        private void TryToConnect(ExternalProgramExecutor externalProgramExecutor, string resetDatabaseScript, IDatabasePersistenceConfiguration configuration)
        {

            this._GenericDatabaseInteractor = DBUtilities.ToGenericDatabaseInteractor(configuration, this._Log);
            if (!externalProgramExecutor.IsRunning && externalProgramExecutor.ExitCode != 0)
            {
                throw new AbortException(new InternalAlgorithmException($"docker exited with exitcode {externalProgramExecutor.ExitCode}; StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErrt: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)};"));
            }
            bool connected = false;
            while (!connected)
            {
                connected = this._GenericDatabaseInteractor.IsAvailable();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }



            List<string> tables = this._GenericDatabaseInteractor.GetAllTableNames().ToList();
            if (1 < tables.Count)
            {
                DbConnection connection = this._GenericDatabaseInteractor.GetConnection();
                using DbTransaction tx = connection.BeginTransaction();
                try
                {
                    using DbCommand cmd = connection.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandText = resetDatabaseScript;
                    cmd.ExecuteNonQuery();
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
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
                    using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("task", this._TaskNameStop, this._TestDatabaseFolder);
                    externalProgramExecutor.Configuration.WaitingState = new RunSynchronously();
                    externalProgramExecutor.Run();
                    GUtilities.AssertCondition(externalProgramExecutor.ExitCode == 0, $"Error while stopping test-database using command \"{externalProgramExecutor.CMD}\" due to exitcode {externalProgramExecutor.ExitCode}. StdOut: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdOutLines)}; StdErr: {string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines)}");
                }
                this._Disposed = true;
            }
        }

        public void ResetDatabase()
        {
            if (0 < this.GenericDatabaseInteractor().GetAllTableNames().ToList().Count)
            {
                DbConnection connection = this._GenericDatabaseInteractor.GetConnection();
                using DbTransaction tx = connection.BeginTransaction();
                try
                {
                    using DbCommand cmd = connection.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandText = this._ResetDatabaseScript;
                    cmd.ExecuteNonQuery();
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
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
