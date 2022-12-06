namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class ServerSettings
    {
        public string Domain { get; set; } = "localhost";
        public ushort Port { get; set; } = 9700;
        public ulong RequestsPerMinuteLimit { get; set; } = ulong.MaxValue;
    }
}
