namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IISettingsInterface
    {
        public ServerSettings ServerSettings { get; set; }
        public WebServerSettings WebServerSettings { get; set; }
        public string GetLogFolder();
        public System.Version GetVersion();
        public Environment GetTargetEnvironmentType();
        public string GetProgramName();
       
    }
}
