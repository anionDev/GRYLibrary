using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which removes some not required information of responses for security purposes.
    /// </summary>
    public class Obfuscation : AbstractMiddleware
    {
        /// <inheritdoc>/>
        public Obfuscation(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc>/>
        public override Task Invoke(HttpContext context)
        {
            // TODO change response to either return 200 (Ok) or else 400 (bad request, because it was a request which did not result in 200).
            
            return _Next(context);
        }
    }
}
