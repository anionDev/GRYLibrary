using GRYLibrary.Core.Misc.ConsoleApplication;

namespace GRYLibrary.Core.APIServer.Services.Init
{
    public class NoInitializationService : IInitializationService<ICommandlineParameter>
    {
        public void Initialize(ICommandlineParameter commandlineParameter)
        {
            GRYLibrary.Core.Misc.Utilities.NoOperation();
        }
    }
}
