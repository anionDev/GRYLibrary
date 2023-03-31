namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class WebApplicationFirewallSettings :IWebApplicationFirewallSettings
    {
        public bool Enabled { get; set; } = false;
    }
}