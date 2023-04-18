using System.Xml.Serialization;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IWebAPIConfigurationVariables
    {
        public GeneralApplicationSettings GeneralApplicationSettings { get; set; }
        public WebServerSettings WebServerSettings { get; set; }
    }
    public class WebAPIConfigurationVariables :IWebAPIConfigurationVariables
    {
        public WebAPIConfigurationVariables() { }
        public WebServerSettings WebServerSettings { get; set; }
        public GeneralApplicationSettings GeneralApplicationSettings { get; set; }
    }
}