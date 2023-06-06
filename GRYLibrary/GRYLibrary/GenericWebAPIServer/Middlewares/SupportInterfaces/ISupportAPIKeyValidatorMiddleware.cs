using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.SupportInterfaces
{
    public interface ISupportAPIKeyValidatorMiddleware
    {
        public IAPIKeyValidatorSettings APIKeyValidatorSettings { get; set; }
    }
}
