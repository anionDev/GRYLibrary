using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authentication.KeyCloak
{
    public abstract class KeyCloakAuthenticationConfiguration :IKeyCloakAuthenticationConfiguration
    {
        public bool Enabled { get; set; } = true;

        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
