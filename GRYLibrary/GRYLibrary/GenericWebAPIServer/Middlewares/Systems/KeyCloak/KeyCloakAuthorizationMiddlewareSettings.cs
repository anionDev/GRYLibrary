namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public class KeyCloakAuthorizationMiddlewareSettings :IKeyCloakAuthorizationMiddlewareSettings
    {
        public bool Enabled { get; set; } = false;
    }
}
