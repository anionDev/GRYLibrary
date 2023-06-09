namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authorization
{
    public interface ISupportAuthorizationMiddleware :ISupportedMiddleware
    {
        IAuthorizationConfiguration ConfigurationForAuthorizationMiddleware { get; set; }
    }
}
