using GRYLibrary.Core.Logging.GRYLogger;

namespace GRYLibrary.Core.APIServer.Mid.Logging
{
    public interface IRequestLoggingConfiguration : IMiddlewareConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; }
        public bool LogClientIP { get; set; }
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public uint MaximalLengthofRequestBodies { get; set; }
        public uint MaximalLengthofResponseBodies { get; set; }
    }
}