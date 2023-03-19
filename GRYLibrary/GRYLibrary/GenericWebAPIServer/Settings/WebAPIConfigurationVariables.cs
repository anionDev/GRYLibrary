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
        public WebAPIConfigurationVariables() { }
        public WebServerSettings WebServerSettings { get; set; } 
        public ApplicationSettings ApplicationSettings { get; set; } 
    }
}
