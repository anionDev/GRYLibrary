﻿using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Miscellaneous.Migration;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    public interface IDatabaseManager
    {
        public IList<MigrationInstance> GetAllMigrations();
        public IGenericDatabaseInteractor GetGenericDatabaseInteractor();
    }
}
