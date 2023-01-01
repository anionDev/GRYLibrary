using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationVariables
    {
        public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();
        public WebServerSettings WebServerSettings { get; set; } = new WebServerSettings();
     }
}
