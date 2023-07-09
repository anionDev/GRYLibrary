namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Blacklist
{
    public interface ISupportBlacklistMiddleware :ISupportedMiddleware
    {
        public IBlacklistConfiguration ConfigurationForBlacklistMiddleware { get; set; }
    }
}
