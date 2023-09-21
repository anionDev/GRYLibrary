using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.Aut.KC
{
    public class KeyCloakAuthorizationMiddlewareConfiguration : IKeyCloakAuthorizationMiddlewareConfiguration
    {
        public bool Enabled { get; set; } = true;


        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
