namespace GRYLibrary.Core.APIServer.Mid.Exception
{
    public interface ISupportExceptionManagerMiddleware :ISupportedMiddleware
    {
        public IExceptionManagerConfiguration ConfigurationForExceptionManagerMiddleware { get; set; }
    }
}
