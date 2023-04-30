namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class WebApplicationFirewallSettings :IWebApplicationFirewallSettings
    {
        public bool Enabled { get; set; } = false;
    }
}