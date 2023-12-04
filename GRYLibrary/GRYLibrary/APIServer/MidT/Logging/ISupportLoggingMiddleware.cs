using GRYLibrary.Core.APIServer.MidT.Logging;

namespace GRYLibrary.Core.APIServer.MidT.Exception
{
    public interface ISupportLoggingMiddleware : ISupportedMiddleware
    {
        public ILoggingConfiguration ConfigurationForLoggingMiddleware { get; set; }
    }
}
