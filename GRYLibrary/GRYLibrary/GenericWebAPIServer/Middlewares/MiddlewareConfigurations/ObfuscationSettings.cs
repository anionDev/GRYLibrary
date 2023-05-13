using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class ObfuscationSettings :IObfuscationSettings
    {
        public bool Enabled { get; set; } = false;
    }
}