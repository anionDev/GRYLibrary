namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthenticationMiddlewareSettings :IKeyCloakAuthenticationMiddlewareSettings
    {
        public bool Enabled { get; set; } = false;      
    }
}
