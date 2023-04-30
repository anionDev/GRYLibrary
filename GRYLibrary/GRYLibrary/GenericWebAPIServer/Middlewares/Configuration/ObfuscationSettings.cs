namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class ObfuscationSettings :IObfuscationSettings
    {
        public bool Enabled { get; set; } = false;
    }
}