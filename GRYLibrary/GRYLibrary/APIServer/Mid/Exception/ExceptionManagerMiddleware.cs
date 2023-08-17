using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Exception
{
    /// <summary>
    /// Represents a middleware which handles exceptions.
    /// </summary>
    public class ExceptionManagerMiddleware :AbstractMiddleware
    {
        private readonly IExceptionManagerConfiguration _ExceptionManagerSettings;
        /// <inheritdoc/>
        public ExceptionManagerMiddleware(RequestDelegate next, IExceptionManagerConfiguration exceptionManagerSettings) : base(next)
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
            catch(System.Exception)
            {
                // TODO log exception and requestbody, return 500
                throw;
            }
        }
    }
}