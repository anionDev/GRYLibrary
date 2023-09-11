using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.WAF
{
    public class WebApplicationFirewallConfiguration : IWebApplicationFirewallConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}