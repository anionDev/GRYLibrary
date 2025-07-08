using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using System;

namespace GRYLibrary.Core.APIServer.Services.Database.SupportedDatabases
{
    public class Oracle : IDatabase
    {
        public void Accept(ISupportedDatabaseVisitor visitor)
        {
            visitor.Handle(this);
        }

        public T Accept<T>(ISupportedDatabaseVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public IGenericDatabaseInteractor ToDatabaseInteractor()
        {
            throw new NotSupportedException();
        }
    }
}
