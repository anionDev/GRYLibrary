using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.MidT.DDOS
{
    public class DDOSProtectionConfiguration : IDDOSProtectionConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}