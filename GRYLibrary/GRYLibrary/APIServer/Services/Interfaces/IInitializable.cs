using GRYLibrary.Core.APIServer.Utilities;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IInitializable
    {
        public InitializationState InitializationState { get; }
        public void Initialize();
    }
}
