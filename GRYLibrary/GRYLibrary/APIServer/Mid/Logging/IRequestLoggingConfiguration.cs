using GRYLibrary.Core.APIServer.Mid;
using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.APIServer.Mid.Logging
{
    public interface IRequestLoggingConfiguration :IMiddlewareConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; }
        public bool LogClientIP { get; set; }
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public uint MaximalLengthofBodies { get; set; }
    }
}