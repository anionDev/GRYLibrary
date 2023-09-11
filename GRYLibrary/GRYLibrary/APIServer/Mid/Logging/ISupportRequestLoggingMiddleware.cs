namespace GRYLibrary.Core.APIServer.Mid.Logging
{
    public interface ISupportRequestLoggingMiddleware : ISupportedMiddleware
    {
        public IRequestLoggingConfiguration ConfigurationForRequestLoggingMiddleware { get; }
        //TODO this property cannot be serialized yet. maybe switching to https://github.com/ExtendedXmlSerializer/home can solve serialization-issues.
    }
}
