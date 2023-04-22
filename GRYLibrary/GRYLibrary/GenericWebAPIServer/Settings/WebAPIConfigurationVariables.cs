namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents all configuration-settings which are allowed to be changed in the configuration-file.
    /// </summary>
    public interface IWebAPIConfigurationVariables
    {
        public GeneralApplicationSettings GeneralApplicationSettings { get; set; }
        public WebServerConfiguration WebServerSettings { get; set; }
    }
    /// <inheritdoc cref="IWebAPIConfigurationVariables"/>
    public class WebAPIConfigurationVariables :IWebAPIConfigurationVariables
    {
        public WebServerConfiguration WebServerSettings { get; set; }
        public GeneralApplicationSettings GeneralApplicationSettings { get; set; }
    }
}