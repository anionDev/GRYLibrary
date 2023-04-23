namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents a <see cref="WebAPIConfigurationVariables"/> with a custom configuration-section for application-specific-settings.
    /// </summary>
    public class CustomWebAPIConfigurationVariables<T> :WebAPIConfigurationVariables where T : new()
    {
        public T ApplicationSpecificSettings { get; set; } = new T();
    }
}
