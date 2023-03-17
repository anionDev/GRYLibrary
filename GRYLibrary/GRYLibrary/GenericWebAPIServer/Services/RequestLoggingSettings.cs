namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class RequestLoggingSettings : IRequestLoggingSettings
    {
        public bool Enabled { get; set; } = false;
    }
}
