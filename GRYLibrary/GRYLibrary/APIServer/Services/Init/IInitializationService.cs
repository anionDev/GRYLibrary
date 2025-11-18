using GRYLibrary.Core.APIServer.Utilities;
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
        where CommandlineParameter : ICommandlineParameter
    {
        public void Initialize(CommandlineParameter commandlineParameter);
    }
}
