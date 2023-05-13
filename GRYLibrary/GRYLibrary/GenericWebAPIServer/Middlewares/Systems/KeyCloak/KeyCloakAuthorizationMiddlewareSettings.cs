using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthorizationMiddlewareSettings :IKeyCloakAuthorizationMiddlewareSettings
    {
        public bool Enabled { get; set; } = false;
    }
}
