namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public interface IBackgroundService
    {
        public void StartAsync();

        public void Stop();
    }
}
