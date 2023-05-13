using Microsoft.AspNetCore.Http;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    public abstract class AuthorizationMiddleware :AbstractMiddleware
    {
        protected AuthorizationMiddleware(RequestDelegate next) : base(next)
        {
        }
    }
}
