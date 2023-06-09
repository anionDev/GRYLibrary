using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.DDOSProtectionMiddleware
{
    public class DDOSProtectionConfiguration :IDDOSProtectionConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}