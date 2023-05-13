using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public interface IMiddlewareSettings
    {
        public bool Enabled { get; set; }
    }
}