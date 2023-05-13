using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class WebApplicationFirewallSettings :IWebApplicationFirewallSettings
    {
        public bool Enabled { get; set; } = false;
    }
}