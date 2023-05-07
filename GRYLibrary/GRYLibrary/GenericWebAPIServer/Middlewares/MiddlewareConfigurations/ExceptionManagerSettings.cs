using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class ExceptionManagerSettings :IExceptionManagerSettings
    {
        public bool Enabled { get; set; } = false;
        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}