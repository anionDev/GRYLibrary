
namespace GRYLibrary.Core.APIServer.MidT.RLog
{
    public interface ISupportLoggingMiddleware : ISupportedMiddleware
    {
        public ILoggingConfiguration ConfigurationForLoggingMiddleware { get;  }
    }
}
