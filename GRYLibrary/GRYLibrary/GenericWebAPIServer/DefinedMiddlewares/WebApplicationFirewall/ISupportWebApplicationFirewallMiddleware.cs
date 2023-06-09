namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.WebApplicationFirewall
{
    public interface ISupportWebApplicationFirewallMiddleware :ISupportedMiddleware
    {
        public IWebApplicationFirewallConfiguration ConfigurationForWebApplicationFirewall { get; set; }
    }
}
