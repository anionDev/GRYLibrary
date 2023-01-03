using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IWebAPIConfigurationVariables
    {
        public ApplicationSettings ApplicationSettings { get; set; } 
        public WebServerSettings WebServerSettings { get; set; }
    }
    public class WebAPIConfigurationVariables: IWebAPIConfigurationVariables
    {
        public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();
        public WebServerSettings WebServerSettings { get; set; } = new WebServerSettings();
    }
}
