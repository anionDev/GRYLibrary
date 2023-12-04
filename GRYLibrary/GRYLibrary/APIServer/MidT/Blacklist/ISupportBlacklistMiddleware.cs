namespace GRYLibrary.Core.APIServer.MidT.Blacklist
{
    public interface ISupportBlacklistMiddleware : ISupportedMiddleware
    {
        public IBlacklistConfiguration ConfigurationForBlacklistMiddleware { get; }
    }
}
