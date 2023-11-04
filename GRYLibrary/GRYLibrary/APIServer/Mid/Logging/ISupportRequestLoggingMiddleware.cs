namespace GRYLibrary.Core.APIServer.Mid.Logging
{
    public interface ISupportRequestLoggingMiddleware : ISupportedMiddleware
    {
        public IRequestLoggingConfiguration ConfigurationForRequestLoggingMiddleware { get; }
    }
}
