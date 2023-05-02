namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public interface IAPIKeyValidatorSettings :IMiddlewareSettings
    {
        public bool APIKeyIsValid(string apiKey,string method, string route);
    }
}