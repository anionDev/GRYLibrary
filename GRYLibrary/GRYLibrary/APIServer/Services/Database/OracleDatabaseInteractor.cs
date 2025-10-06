﻿using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Services.Database
{
    public class OracleDatabaseInteractor : GenericDatabaseInteractor
    {

        public OracleDatabaseInteractor(IDatabasePersistenceConfiguration configuration, IGRYLog log) : base(configuration, log)
        {
        }
        public override string CreateSQLStatementForCreatingMigrationMaintenanceTableIfNotExist(string tableName)
        {
            throw new NotImplementedException();
        }

        public override string CreateSQLStatementForGetAllTableNames()
        {
            throw new NotImplementedException();
        }

        public override string EscapePasswordInConnectionString(string connectionString)
        {
            throw new NotImplementedException();
        }

        public override string GetSQLStatementForRunningMigration(string migrationContent, string migrationTableName, string migrationName, DateTimeOffset now)
        {
            throw new NotImplementedException();
        }

        public override string GetSQLStatementForSelectMigrationMaintenanceTableContent(string migrationTableName)
        {
            throw new NotImplementedException();
        }
        public override void Accept(IGenericDatabaseInteractorVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IGenericDatabaseInteractorVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override DbCommand CreateCommand(string sql)
        {
            var connection = GetConnection();
            throw new NotImplementedException();
        }

        protected override DbConnection CreateNewConnectionObject(string connectionString)
        {
            throw new NotImplementedException();
        }

        public override DbParameter GetParameter(string parameterName, object? value, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
