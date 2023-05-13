using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    public abstract class AuthenticationMiddleware :AbstractMiddleware
    {
        protected AuthenticationMiddleware(RequestDelegate next) : base(next)
        {
        }
    }
}
