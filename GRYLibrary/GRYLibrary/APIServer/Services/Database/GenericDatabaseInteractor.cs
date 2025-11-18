using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                    Thread.Sleep(TimeSpan.FromSeconds(15));//connected. wait some seconds and before checking again if the database is still available.
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));//not connected. wait a few seconds until checking again if the database is avbailable.
                }
            }
            this._Connection.Dispose();
        }
        private DbConnection CreateConnection()
        {
            string connectionStringForLog = this._Configuration.DatabaseConnectionString;
            if (this._Configuration.EscapePasswordInLog)
            {
                connectionStringForLog = this.EscapePassword(this._Configuration.DatabaseConnectionString);
            }
            this.Log.Log($"Try to create database-connection using connection-string \"{connectionStringForLog}\".", LogLevel.Information);
            DbConnection conn = this.CreateNewConnectionObject(this._Configuration.DatabaseConnectionString);
            conn.Open();
            return conn;
        }

        private string EscapePassword(string databaseConnectionString)
        {
            string output = Regex.Replace(databaseConnectionString, @"Password=[^;]*", "Password=********");
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
            if (this.TryGetConnection(out DbConnection? connection, out _))
            {
                return connection!;
            }
            else
            {
                string message = "Database not available.";
                this.Log.Log(message, LogLevel.Warning);
                throw new DependencyNotAvailableException(message);
            }
        }
        public bool TryGetConnection(out DbConnection? connection, out Exception? err)
        {
            try
            {
                GRYLibrary.Core.Misc.Utilities.AssertCondition(this.IsConnected(), "Not connected");
                connection = this.GetConnectionInternal();
                err = null;
                return true;
            }
            catch (Exception e)
            {
                connection = null;
                err = e;
                return false;
            }
        }

        public bool IsConnected()
        {
            if (this.GetConnectionInternal() == null)
            {
                return false;
            }
            else
            {
                bool result = this.GetConnectionInternal().State == System.Data.ConnectionState.Open;
                return result;
            }
        }

        public (bool, Exception?) IsAvailable()
        {
            try
            {
                GUtilities.AssertCondition(this.IsConnected(), "Database is not connected");
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
            catch (Exception e)
            {
                return (false, e);
            }
        }

        public IEnumerable<string> GetAllTableNames()
        {
            IList<string> result = new List<string>();
            using (DbCommand cmd = this.CreateCommand(this.CreateSQLStatementForGetAllTableNames()))
            {
                using DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
            return result;
        }


        public void Dispose()
        {
            this._ThreadEnabled = false;
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
