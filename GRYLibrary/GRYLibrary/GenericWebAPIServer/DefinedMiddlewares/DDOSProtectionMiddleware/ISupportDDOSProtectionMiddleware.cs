namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.DDOSProtectionMiddleware
{
    public interface ISupportDDOSProtectionMiddleware :ISupportedMiddleware
    {
        IDDOSProtectionConfiguration ConfigurationForDDOSProtection { get; set; }
    }
}
