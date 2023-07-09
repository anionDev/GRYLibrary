namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Obfuscation
{
    public interface ISupportObfuscationMiddleware :ISupportedMiddleware
    {
        public IObfuscationConfiguration ConfigurationForObfuscationMiddleware { get; set; }
    }
}
