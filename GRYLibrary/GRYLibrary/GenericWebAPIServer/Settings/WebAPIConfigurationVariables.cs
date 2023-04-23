namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents all configuration-settings which are allowed to be changed in the configuration-file.
    /// </summary>
    public class WebAPIConfigurationVariables<CustomConfigurationType>
        where CustomConfigurationType : new()
    {
        public WebServerConfiguration WebServerSettings { get; set; }
        public GeneralApplicationSettings GeneralApplicationSettings { get; set; }
        public CustomConfigurationType ApplicationSpecificSettings { get; set; } = new CustomConfigurationType();
    }
}