namespace GRYLibrary.Core.APIServer.Services.Init
{
    public class NoInitializationService : IInitializationService
    {
        public void Initialize()
        {
            GRYLibrary.Core.Misc.Utilities.NoOperation();
        }
    }
}
