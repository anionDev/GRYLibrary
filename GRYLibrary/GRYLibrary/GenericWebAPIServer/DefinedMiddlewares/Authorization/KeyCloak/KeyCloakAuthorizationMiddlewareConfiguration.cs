using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authorization.KeyCloak
{
    public abstract class KeyCloakAuthorizationMiddlewareConfiguration :IKeyCloakAuthorizationMiddlewareConfiguration
    {
        public bool Enabled { get; set; } = true;


        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}
