using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which logs the requests.
    /// </summary>
    public class Log : AbstractMiddleware
    {
        /// <inheritdoc>/>
        public Log(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc>/>
        public override Task Invoke(HttpContext context)
        {
            // TODO log request.route, request.sourceip, response.statuscode, duration of creating response (nothing else)

            return _Next(context);
        }
    }
}
