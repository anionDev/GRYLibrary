namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public interface ISupportAuthorizationMiddleware : ISupportedMiddleware
    {
        public IAuthorizationConfiguration ConfigurationForAuthorizationMiddleware { get; }
    }
}
