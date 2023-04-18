using GRYLibrary.Core.GenericWebAPIServer.Settings;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class CustomWebAPIConfigurationVariables <T>:WebAPIConfigurationVariables
    {
       public T ApplicationSpecificSettings { get; set; }
    }
}
