namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class RequestCounterSettings :IRequestCounterSettings
    {
        public bool Enabled { get; set; } = false;
    }
}