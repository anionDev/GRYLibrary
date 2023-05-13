using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    /// <summary>
    /// Represents a middleware which handles exceptions.
    /// </summary>
    public class ExceptionManager :AbstractMiddleware
    {
        private readonly IExceptionManagerSettings _ExceptionManagerSettings;
        /// <inheritdoc/>
        public ExceptionManager(RequestDelegate next, IExceptionManagerSettings exceptionManagerSettings) : base(next)
        {
            this._ExceptionManagerSettings = exceptionManagerSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            try
            {
                // TODO if response.statuscode is 500 then log requestbody
                return this._Next(context);
            }
            catch(Exception)
            {
                // TODO log exception and requestbody, return 500
                throw;
            }
        }
    }
}