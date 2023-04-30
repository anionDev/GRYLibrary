namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class APIKeyValidatorSettings :IAPIKeyValidatorSettings
    {
        public bool Enabled { get; set; } = false;
    }
}