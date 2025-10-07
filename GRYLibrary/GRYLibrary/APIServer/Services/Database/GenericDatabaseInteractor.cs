using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public abstract class GenericDatabaseInteractor : IGenericDatabaseInteractor
    {
        private readonly IDatabasePersistenceConfiguration _Configuration;
        private readonly ReaderWriterLockSlim _Lock = new();
        private DbConnection _Connection;
        private readonly Thread _ConnectionThread;
        private bool _ThreadEnabled = true;//TODO make this variable threadsafe
        public IGRYLog Log { get; private set; }
        public GenericDatabaseInteractor(IDatabasePersistenceConfiguration configuration, IGRYLog log)
        {
            this._Configuration = configuration;
            this.Log = log;
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
            return this.GetParameter(parameterName, value, value.GetType());
        }
        private void StartTryToConnectScheduler()
        {
            while (_ThreadEnabled)
            {

                try
                {
                    this._Lock.EnterUpgradeableReadLock();
                    try
                    {
                        if (!this.IsAvailable())
                        {
                            this._Lock.EnterWriteLock();
                            try
                            {
                                this._Connection?.Dispose();
                                this._Connection = this.CreateConnection();
                            }
                            catch
                            {
                                throw;
                            }
                            finally
                            {
                                this._Lock.ExitWriteLock();
                            }
                        }
                    }
                    finally
                    {
                        this._Lock.ExitUpgradeableReadLock();
                    }
                    Thread.Sleep(TimeSpan.FromMinutes(1));//connected
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));//not connected
                }
            }
            this._Connection.Dispose();
        }
        private DbConnection CreateConnection()
        {
            DbConnection conn = this.CreateNewConnectionObject(this._Configuration.DatabaseConnectionString);
            conn.Open();
            return conn;
        }
        private DbConnection GetConnectionInternal()
        {
            this._Lock.EnterReadLock();
            try
            {
                return this._Connection!;
            }
            finally
            {
                this._Lock.ExitReadLock();
            }
        }
        public DbConnection GetConnection()
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(this.IsConnected(), "Not connected");
            return this.GetConnectionInternal();
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
                if (!result)
                {
                    int i = 3;
                }
                return result;
            }
        }

        internal bool IsAvailable()
        {
            bool result;
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
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
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

        bool IGenericDatabaseInteractor.IsAvailable()
        {
            return this.IsAvailable();
        }

        public void Dispose()
        {
            _ThreadEnabled = false;
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
