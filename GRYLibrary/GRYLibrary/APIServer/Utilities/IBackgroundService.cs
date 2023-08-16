namespace GRYLibrary.Core.APIServer.Utilities
{
    public interface IBackgroundService
    {
        public void StartAsync();
        public void Stop();
    }
}
