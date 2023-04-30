namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class DDOSProtectionSettings :IDDOSProtectionSettings
    {
        public bool Enabled { get; set; } = false;
    }
}