using GRYLibrary.Core.APIServer.MidT.Logging;
using GRYLibrary.Core.Logging.GRYLogger;

namespace GRYLibrary.Core.APIServer.Mid.DLog
{
    public interface IRequestLoggingConfiguration : ILoggingConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; }
        public bool LogClientIP { get; set; }
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public uint MaximalLengthofRequestBodies { get; set; }
        public uint MaximalLengthofResponseBodies { get; set; }
    }
}