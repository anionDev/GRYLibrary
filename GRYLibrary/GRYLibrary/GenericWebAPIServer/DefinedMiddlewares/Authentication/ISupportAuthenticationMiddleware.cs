namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authentication
{
    public interface ISupportAuthenticationMiddleware :ISupportedMiddleware
    {
        public IAuthenticationConfiguration ConfigurationForAuthenticationMiddleware { get; set; }
    }
}
