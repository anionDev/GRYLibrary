using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.APIServer.Settings
{
    public interface IServiceConfiguration
    {
        public bool Enabled { get; set; }
        public GRYLogConfiguration LogConfiguration { get; set; }
    }
}
