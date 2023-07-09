using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public interface IMiddlewareConfiguration
    {
        public bool Enabled { get; set; }
        public ISet<FilterDescriptor> GetFilter();
    }
}