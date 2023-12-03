namespace GRYLibrary.Core.APIServer.Mid.Blacklist
{
    public interface ISupportBlacklistMiddleware : ISupportedMiddleware
    {
        public IBlacklistConfiguration ConfigurationForBlacklistMiddleware { get;  }
    }
}
