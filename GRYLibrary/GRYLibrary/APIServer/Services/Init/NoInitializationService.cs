using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.Misc.ConsoleApplication;

namespace GRYLibrary.Core.APIServer.Services.Init
{
    public class NoInitializationService : IInitializationService<ICommandlineParameter>
    {
        public InitializationState GetInitializationState()
        {
            return  new Initialized();
        }

        public void Initialize(ICommandlineParameter commandlineParameter)
        {
            Misc.Utilities.NoOperation();
        }
    }
}
