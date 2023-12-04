using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.MidT.Logging
{
    public class LoggingConfiguration : ILoggingConfiguration
    {
        public bool Enabled { get; set; } = true;

        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}