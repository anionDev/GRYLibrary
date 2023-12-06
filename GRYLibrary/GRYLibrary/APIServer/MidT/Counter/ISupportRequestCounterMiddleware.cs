namespace GRYLibrary.Core.APIServer.MidT.Counter
{
    public interface ISupportRequestCounterMiddleware : ISupportedMiddleware
    {
        IRequestCounterConfiguration ConfigurationForRequestCounterMiddleware { get; }
    }
}
