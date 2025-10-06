using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc.Migration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
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

        public IGRYLog Log { get; private set; }

        public GenericDatabaseInteractor(IDatabasePersistenceConfiguration configuration, IGRYLog log)
        {
            _Configuration = configuration;
            Log = log;
            _ConnectionThread = new Thread(StartTryToConnectScheduler);
            _ConnectionThread.Start();
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
        public abstract void Dispose();
        public abstract DbParameter GetParameter(string parameterName, object? value, Type type);

        public DbParameter GetParameter(string parameterName, object value)
        {
            GUtilities.AssertCondition(value != null, $"value for parameter {parameterName} is null, so a speicfic type for it must be set.");
            return GetParameter(parameterName, value, value.GetType());
        }
        private void StartTryToConnectScheduler()
        {
            bool enabled = false;
            while (enabled)
            {

                try
                {
                    _Lock.EnterUpgradeableReadLock();
                    try
                    {
                        if (!IsAvailable())
                        {
                            _Lock.EnterWriteLock();
                            try
                            {
                                _Connection?.Dispose();
                                _Connection = CreateConnection();
                                Thread.Sleep(TimeSpan.FromMinutes(1));//connected
                            }
                            finally
                            {
                                _Lock.ExitWriteLock();
                            }
                        }
                    }
                    finally
                    {
                        _Lock.ExitUpgradeableReadLock();
                    }
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));//not connected
                }
            }
        }
        private DbConnection CreateConnection()
        {
            var conn = CreateNewConnectionObject(_Configuration.DatabaseConnectionString);
            conn.Open();
            return conn;
        }
        private DbConnection GetConnectionInternal()
        {
            _Lock.EnterReadLock();
            try
            {
                return _Connection!;
            }
            finally
            {
                _Lock.ExitReadLock();
            }
        }
        public DbConnection GetConnection()
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(IsConnected());
            return GetConnectionInternal();
        }

        public bool IsConnected()
        {
            return GetConnectionInternal().State == System.Data.ConnectionState.Open;
        }

        internal bool IsAvailable()
        {
            try
            {
                GUtilities.AssertCondition(IsConnected());
                using (DbDataReader reader = this.CreateCommand("select 1;").ExecuteReader())
                {
                    GUtilities.AssertCondition(reader.HasRows, "Test-statement did not return any row. So database-connection is not ready.");
                    while (reader.Read())
                    {
                        GUtilities.NoOperation(); // Just to ensure that we can read from the reader without any exceptions
                    }
                }
                return true;
            }
            catch
            {
                return false;
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

        bool IGenericDatabaseInteractor.IsAvailable()
        {
            return this.IsAvailable();
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
