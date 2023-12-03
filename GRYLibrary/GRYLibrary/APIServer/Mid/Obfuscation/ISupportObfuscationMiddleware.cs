namespace GRYLibrary.Core.APIServer.Mid.Obfuscation
{
    public interface ISupportObfuscationMiddleware : ISupportedMiddleware
    {
        public IObfuscationConfiguration ConfigurationForObfuscationMiddleware { get; }
    }
}
