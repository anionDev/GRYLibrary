namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class ExceptionManagerSettings :IExceptionManagerSettings
    {
        public bool Enabled { get; set; } = false;
    }
}