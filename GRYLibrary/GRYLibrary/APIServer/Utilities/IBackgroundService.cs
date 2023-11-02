using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public interface IBackgroundService
    {
        public void StartAsync();
        public Task Stop();
    }
}
