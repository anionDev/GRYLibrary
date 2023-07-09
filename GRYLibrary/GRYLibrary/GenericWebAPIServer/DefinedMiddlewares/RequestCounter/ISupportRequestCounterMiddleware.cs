namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestCounter
{
    public interface ISupportRequestCounterMiddleware :ISupportedMiddleware
    {
        IRequestCounterConfiguration ConfigurationForRequestCounterMiddleware { get; set; }
    }
}
