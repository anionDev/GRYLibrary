namespace GRYLibrary.Core.APIServer.MidT.DDOS
{
    public interface ISupportDDOSProtectionMiddleware : ISupportedMiddleware
    {
        IDDOSProtectionConfiguration ConfigurationForDDOSProtection { get; }
    }
}
