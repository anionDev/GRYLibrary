using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.SupportInterfaces
{
    public interface ISupportRequestLoggingMiddleware:ISupportedMiddleware
    {
        public IRequestLoggingSettings RequestLoggingSettings { get; set; }
        //TODO this property cannot be serialized yet. maybe switching to https://github.com/ExtendedXmlSerializer/home can solve serialization-issues.
    }
}
