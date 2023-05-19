using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class DDOSProtectionSettings :IDDOSProtectionSettings
    {
        public bool Enabled { get; set; } = false;     
    }
}