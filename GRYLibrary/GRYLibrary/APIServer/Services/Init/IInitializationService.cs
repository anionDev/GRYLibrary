using GRYLibrary.Core.Misc.ConsoleApplication;

namespace GRYLibrary.Core.APIServer.Services.Init
{
    /// <summary>
    /// Does business-logic-related initialization.
    /// </summary>
    public interface IInitializationService<CommandlineParameter>
        where CommandlineParameter: ICommandlineParameter
    {
        public void Initialize(CommandlineParameter commandlineParameter);
    }
}
