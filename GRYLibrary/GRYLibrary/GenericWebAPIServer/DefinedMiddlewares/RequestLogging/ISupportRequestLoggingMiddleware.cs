namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging
{
    public interface ISupportRequestLoggingMiddleware :ISupportedMiddleware
    {
        public IRequestLoggingConfiguration ConfigurationForRequestLoggingMiddleware { get; set; }
        //TODO this property cannot be serialized yet. maybe switching to https://github.com/ExtendedXmlSerializer/home can solve serialization-issues.
    }
}
