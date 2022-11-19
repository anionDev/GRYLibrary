namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class ServerSettings
    {
        public string Domain { get; set; }
        public ushort Port { get; set; }
        public ulong RequestsPerMinuteLimit { get; set; }
    }
}
