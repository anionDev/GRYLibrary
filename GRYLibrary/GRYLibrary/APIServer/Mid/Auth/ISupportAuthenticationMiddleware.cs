namespace GRYLibrary.Core.APIServer.Mid.Auth
{
    public interface ISupportAuthenticationMiddleware : ISupportedMiddleware
    {
        public IAuthenticationConfiguration ConfigurationForAuthenticationMiddleware { get;  }
    }
}
