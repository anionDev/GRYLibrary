namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class BlacklistProvider :IBlacklistProvider
    {
        public bool Enabled { get; set; } = false;
    }
}