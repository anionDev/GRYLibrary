namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class CustomWebAPIConfigurationVariables<T> :WebAPIConfigurationVariables where T : new()
    {
        public T ApplicationSpecificSettings { get; set; } = new T();
    }
}
