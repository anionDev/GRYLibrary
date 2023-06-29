using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging
{
    public interface IRequestLoggingConfiguration :IMiddlewareConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; }
        public bool LogClientIP { get; set; }
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public uint MaximalLengthofBodies { get; set; }
    }
}