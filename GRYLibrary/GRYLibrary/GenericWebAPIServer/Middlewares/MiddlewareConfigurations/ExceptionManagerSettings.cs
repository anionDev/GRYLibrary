using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class ExceptionManagerSettings :IExceptionManagerSettings
    {
        public bool Enabled { get; set; } = false;
    }
}