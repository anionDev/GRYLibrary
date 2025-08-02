namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IInitializable
    {
        public bool IsInitialized { get; }
        public void Initialize();
    }
}
