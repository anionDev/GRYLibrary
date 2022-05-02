using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which handles exceptions.
    /// </summary>
    public class ExceptionManager : AbstractMiddleware
    {
        /// <inheritdoc/>
        public ExceptionManager(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            try
            {
                // TODO if response.statuscode is 500 then log requestbody
                return _Next(context);
            }
            catch (Exception)
            {
                // TODO log exception and requestbody, return 500
                throw;
            }
        }
    }
}
