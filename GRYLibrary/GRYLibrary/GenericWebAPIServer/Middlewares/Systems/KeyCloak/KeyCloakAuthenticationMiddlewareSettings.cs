using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthenticationMiddlewareSettings :IKeyCloakAuthenticationMiddlewareSettings
    {
        public bool Enabled { get; set; } = false;
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }        
    }
}
