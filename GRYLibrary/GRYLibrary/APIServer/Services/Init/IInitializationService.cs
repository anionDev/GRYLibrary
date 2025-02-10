using GRYLibrary.Core.Misc.ConsoleApplication;

namespace GRYLibrary.Core.APIServer.Services.Init
{
    public interface IInitializationService<CommandlineParameter>
        where CommandlineParameter: ICommandlineParameter
    {
        public void Initialize(CommandlineParameter commandlineParameter);
    }
}
