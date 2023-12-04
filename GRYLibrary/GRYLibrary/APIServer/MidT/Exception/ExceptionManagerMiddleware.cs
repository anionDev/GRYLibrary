using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Exception
{
    /// <summary>
    /// Represents a middleware which handles exceptions.
    /// </summary>
    public abstract class ExceptionManagerMiddleware : AbstractMiddleware
    {
        /// <inheritdoc/>
        public ExceptionManagerMiddleware(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            try
            {
                // TODO if response.statuscode is 500 then log requestbody
                return this._Next(context);
            }
            catch (System.Exception)
            {
                // TODO log exception and requestbody, return 500
                throw;
            }
        }
    }
}