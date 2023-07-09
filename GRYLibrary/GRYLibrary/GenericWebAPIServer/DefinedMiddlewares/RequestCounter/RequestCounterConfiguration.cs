using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestCounter
{
    public class RequestCounterConfiguration :IRequestCounterConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}