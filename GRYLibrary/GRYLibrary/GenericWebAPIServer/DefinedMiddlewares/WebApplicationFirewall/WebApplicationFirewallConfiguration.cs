using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.WebApplicationFirewall
{
    public class WebApplicationFirewallConfiguration :IWebApplicationFirewallConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}