using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.WebApplicationFirewall
{
    public class WebApplicationFirewallConfiguration :IWebApplicationFirewallConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}