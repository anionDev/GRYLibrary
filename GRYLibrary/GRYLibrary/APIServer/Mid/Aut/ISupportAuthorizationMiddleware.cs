namespace GRYLibrary.Core.APIServer.Mid.Aut
{
    public interface ISupportAuthorizationMiddleware :ISupportedMiddleware
    {
        IAuthorizationConfiguration ConfigurationForAuthorizationMiddleware { get;  }
    }
}
