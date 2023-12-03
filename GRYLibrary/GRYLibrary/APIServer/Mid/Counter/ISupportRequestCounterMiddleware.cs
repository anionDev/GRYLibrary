namespace GRYLibrary.Core.APIServer.Mid.Counter
{
    public interface ISupportRequestCounterMiddleware : ISupportedMiddleware
    {
        IRequestCounterConfiguration ConfigurationForRequestCounterMiddleware { get;  }
    }
}
