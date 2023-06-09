using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Obfuscation
{
    public class ObfuscationConfiguration :IObfuscationConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}