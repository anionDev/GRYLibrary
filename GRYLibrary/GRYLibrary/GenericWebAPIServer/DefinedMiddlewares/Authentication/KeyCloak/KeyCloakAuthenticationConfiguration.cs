using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authentication.KeyCloak
{
    public abstract class KeyCloakAuthenticationConfiguration :IKeyCloakAuthenticationConfiguration
    {
        public bool Enabled { get; set; } = true;

        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}
