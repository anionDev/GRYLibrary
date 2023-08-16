namespace GRYLibrary.Core.APIServer.Mid.DDOS
{
    public interface ISupportDDOSProtectionMiddleware :ISupportedMiddleware
    {
        IDDOSProtectionConfiguration ConfigurationForDDOSProtection { get; set; }
    }
}
