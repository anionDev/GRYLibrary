using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.Aut.Def
{
    public class DefaultAuthorizationConfiguration : IAuthorizationConfiguration
    {
        public bool Enabled { get; set; }

        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
