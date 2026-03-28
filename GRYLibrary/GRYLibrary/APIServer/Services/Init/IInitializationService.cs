using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.APIServer.Verbs;
using GRYLibrary.Core.Misc.ConsoleApplication;

namespace GRYLibrary.Core.APIServer.Services.Init
{
    public interface IInitializationService
    {
        public InitializationState GetInitializationState();
    }
    /// <summary>
    /// Does business-logic-related initialization.
    /// </summary>
    public interface IInitializationService<CommandlineParameter>: IInitializationService
        where CommandlineParameter : RunServer
    {
        public void Initialize(CommandlineParameter commandlineParameter);
    }
}
