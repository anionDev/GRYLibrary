namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.ExceptionManager
{
    public interface ISupportExceptionManagerMiddleware :ISupportedMiddleware
    {
        public IExceptionManagerConfiguration ConfigurationForExceptionManagerMiddleware { get; set; }
    }
}
