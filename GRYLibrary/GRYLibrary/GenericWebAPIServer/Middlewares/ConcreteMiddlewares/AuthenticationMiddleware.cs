using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    public abstract class AuthenticationMiddleware :AbstractMiddleware
    {
        protected AuthenticationMiddleware(RequestDelegate next) : base(next)
        {
        }
    }
}
