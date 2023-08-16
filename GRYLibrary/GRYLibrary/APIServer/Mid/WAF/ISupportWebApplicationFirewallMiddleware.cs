namespace GRYLibrary.Core.APIServer.Mid.WAF
{
    public interface ISupportWebApplicationFirewallMiddleware :ISupportedMiddleware
    {
        public IWebApplicationFirewallConfiguration ConfigurationForWebApplicationFirewall { get; set; }
    }
}
