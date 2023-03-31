namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class BlacklistProvider :IBlacklistProvider
    {
        public bool Enabled { get; set; } = false;
    }
}