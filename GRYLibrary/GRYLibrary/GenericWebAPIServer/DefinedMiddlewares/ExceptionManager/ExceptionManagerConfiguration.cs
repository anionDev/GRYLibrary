﻿using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.ExceptionManager
{
    public class ExceptionManagerConfiguration :IExceptionManagerConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}