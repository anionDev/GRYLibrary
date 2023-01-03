namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebServerSettings
    {
        public string SwaggerDocumentName { get; set; } = "APISpecification";
        public ushort Port { get; set; } = 4422;
        public string APIRoutePrefix { get; set; } = "API";
    }
}
