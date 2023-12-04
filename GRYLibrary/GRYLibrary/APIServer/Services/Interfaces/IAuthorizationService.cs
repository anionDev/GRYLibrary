namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IAuthorizationService
    {
        public bool IsAuthorized(string action);
        public bool IsAuthorized(string action, string secret);
    }
}
