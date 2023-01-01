using GRYLibrary.Core.GenericWebAPIServer.Middlewares;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationValues
    {
        internal IGeneralLogger Logger;
        public WebAPIConfigurationConstants WebAPIConfigurationConstants { get; set; }
        public WebAPIConfigurationVariables WebAPIConfigurationVariables { get; set; }
    }
}
