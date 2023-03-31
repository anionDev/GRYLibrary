namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class DDOSProtectionSettings :IDDOSProtectionSettings
    {
        public bool Enabled { get; set; } = false;
    }
}