using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.Auth.Def
{
    public class DefaultAuthenticationConfiguration : IDefaultAuthenticationConfiguration
    {
        public bool Enabled { get; set; }

        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
