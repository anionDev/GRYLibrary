using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.MidT.RLog
{
    /// <summary>
    /// Configuration for <see cref="LoggingMiddleware"/>
    /// </summary>
    public class LoggingConfiguration : ILoggingConfiguration
    {
        public bool Enabled { get; set; } = true;

        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}