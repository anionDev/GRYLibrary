using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public abstract class GenericDatabaseInteractor : IGenericDatabaseInteractor
    {
        private readonly IDatabasePersistenceConfiguration _Configuration;
        private readonly object _Lock = new object();
        private DbConnection _Connection;
        private readonly Thread _ConnectionThread;
        private bool _ThreadEnabled = true;//TODO make this variable threadsafe
        private bool _ThreadRunning = false;//TODO make this variable threadsafe

        public IGRYLog Log { get; private set; }
        private bool _LogConnectionErrors = false;
        public GenericDatabaseInteractor(IDatabasePersistenceConfiguration configuration, IGRYLog log)
        {
            this.Log = log;
            this._Configuration = configuration;
            this._ConnectionThread = new Thread(this.StartTryToConnectScheduler);
            this._ConnectionThread.Start();
        }

        protected abstract DbConnection CreateNewConnectionObject(string connectionString);
        public abstract DbCommand CreateCommand(string sql);
        public abstract string EscapePasswordInConnectionString(string connectionString);
        public abstract string CreateSQLStatementForGetAllTableNames();
        public abstract string CreateSQLStatementForCreatingMigrationMaintenanceTableIfNotExist(string tableName);
        public abstract string GetSQLStatementForSelectMigrationMaintenanceTableContent(string migrationTableName);
        public abstract string GetSQLStatementForRunningMigration(string migrationContent, string migrationTableName, string migrationName, DateTimeOffset now);
        public abstract void Accept(IGenericDatabaseInteractorVisitor visitor);
        public abstract T Accept<T>(IGenericDatabaseInteractorVisitor<T> visitor);

        public abstract DbParameter GetParameter(string parameterName, object? value, Type type);

        public DbParameter GetParameter(string parameterName, object value)
        {
            GUtilities.AssertCondition(value != null, $"value for parameter {parameterName} is null, so a speicfic type for it must be set.");
            return this.GetParameter(parameterName, value, value!.GetType());
        }
        private void StartTryToConnectScheduler()
        {
            this._ThreadRunning = true;
            while (this._ThreadEnabled)
            {
                try
                {
                    lock (this._Lock)
                    {
                        (bool isAvailable, Exception? exc) = this.IsAvailable();
                        if (!this.IsAvailable().Item1)
                        {
                            try
                            {
                                this._Connection?.Dispose();
                                this._Connection = this.CreateConnection();
                            }
                            catch (Exception ex)
                            {
                                if (this._LogConnectionErrors)
                                {
                                    this.Log.Log("Error while connecting to database.", ex);
                                }
                                throw;
                            }
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(5));//connected. wait some seconds and before checking again if the database is still available.
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));//not connected. wait a few seconds until checking again if the database is avbailable.
                }
            }
            this._Connection.Dispose();
            this._ThreadRunning = false;
        }
        private DbConnection CreateConnection()
        {
            string connectionStringForLog = this._Configuration.DatabaseConnectionString;
            if (this._Configuration.EscapePasswordInLog)
            {
                connectionStringForLog = this.EscapePassword(this._Configuration.DatabaseConnectionString);
            }
            GRYLibrary.Core.Misc.Utilities.AssertCondition(Regex.IsMatch(connectionStringForLog, @"Database=[^;]+", RegexOptions.IgnoreCase), $"Connectionstring \"{connectionStringForLog}\" does not contain a databasename-specification.");
            this.Log.Log($"Try to create database-connection using connection-string \"{connectionStringForLog}\".", LogLevel.Information);
            DbConnection conn = this.CreateNewConnectionObject(this._Configuration.DatabaseConnectionString);
            conn.Open();
            return conn;
        }

        private string EscapePassword(string databaseConnectionString)
        {
            string output = Regex.Replace(databaseConnectionString, @"Password=[^;]+", "Password=********");
            return output;
        }

        private DbConnection GetConnectionInternal()
        {
            DbConnection result = this._Connection;
            lock (this._Lock)
            {
                result = this._Connection!;
            }
            return result;
        }
        public DbConnection GetConnection()
        {
            Exception? error;
            if (this.TryGetConnection(out DbConnection? connection, out error))
            {
                return connection!;
            }
            else
            {
                string message = "Database not available.";
                this.Log.Log(message, LogLevel.Warning);
                if (error == null)
                {
                    throw new DependencyNotAvailableException(message);
                }
                else
                {
                    throw new DependencyNotAvailableException(message, error);
                }
            }
        }
        public bool TryGetConnection(out DbConnection? connection, out Exception? err)
        {
            try
            {
                Exception? connectedException;
                bool isConnected = this.IsConnected(out connectedException);
                if (isConnected)
                {
                    err = null;
                    connection = this.GetConnectionInternal();
                    return true;
                }
                else
                {
                    if (connectedException == null)
                    {
                        err = new DependencyNotAvailableException("Not connected");
                    }
                    else
                    {
                        err = new DependencyNotAvailableException("Not connected", connectedException);
                    }
                    connection = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                connection = null;
                err = e;
                return false;
            }
        }

        public bool IsConnected(out Exception? exception)
        {
            if (this.GetConnectionInternal() == null)
            {
                exception = new DependencyNotAvailableException("Connection is null.");
                return false;
            }
            else
            {
                ConnectionState state = this.GetConnectionInternal().State;
                bool result = state == System.Data.ConnectionState.Open;
                if (result)
                {
                    exception = null;
                    return true;
                }
                else
                {
                    exception = new DependencyNotAvailableException($"Connection-state is \"{state}\".");
                    return false;
                }
            }
        }

        public (bool, Exception?) IsAvailable()
        {
            try
            {
                var connected = this.IsConnected(out Exception? connectionExceptionN);
                if (connected)
                {
                    GUtilities.AssertCondition(connected, "Database is not connected.");
                    using (DbDataReader reader = this.CreateCommand("select 1;").ExecuteReader())
                    {
                        GUtilities.AssertCondition(reader.HasRows, "Test-statement did not return any row. So database-connection is not ready.");
                        while (reader.Read())
                        {
                            GUtilities.NoOperation(); // Just to ensure that we can read from the reader without any exceptions
                        }
                    }
                    return (true, null);
                }
                else
                {
                    Exception connectionException = GRYLibrary.Core.Misc.Utilities.AssertNotNull(connectionExceptionN, "Unknown connection-exception.");
                    return (false, connectionException);
                }

            }
            catch (Exception e)
            {
                return (false, e);
            }
        }

        public IEnumerable<string> GetAllTableNames()
        {
            IList<string> result = [];
            string sql = this.CreateSQLStatementForGetAllTableNames();
            using (DbCommand cmd = this.CreateCommand(sql))
            {
                try
                {
                    using DbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
                catch
                {
                    throw;
                }
            }
            return result;
        }


        public void Dispose()
        {
            this._ThreadEnabled = false;
            GUtilities.WaitUntilConditionIsTrue(() => !this._ThreadRunning);
        }

        public void SetLogConnectionAttemptErrors(bool enabled)
        {
            lock (this._Lock)
            {
                this._LogConnectionErrors = enabled;
            }
        }

        public void DoAllMigrations(IList<MigrationInstance> migrations, ITimeService timeService)
        {
            GRYMigrator migrator = new GRYMigrator(timeService, migrations, this);
            migrator.InitializeDatabaseAndMigrateIfRequired();
        }
    }

    public interface IGenericDatabaseInteractorVisitor
    {
        void Handle(MariaDBDatabaseInteractor mariaDBDatabaseInteractor);
        void Handle(OracleDatabaseInteractor oracleDatabaseInteractor);
        void Handle(SQLServerDatabaseInteractor sQLServerDatabaseInteractor);
        void Handle(PostgreSQLDatabaseInteractor postgreSQLDatabaseInteractor);
    }
    public interface IGenericDatabaseInteractorVisitor<T>
    {
        T Handle(MariaDBDatabaseInteractor mariaDBDatabaseInteractor);
        T Handle(OracleDatabaseInteractor oracleDatabaseInteractor);
        T Handle(SQLServerDatabaseInteractor sQLServerDatabaseInteractor);
        T Handle(PostgreSQLDatabaseInteractor postgreSQLDatabaseInteractor);
    }
}
