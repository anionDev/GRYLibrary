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
        protected abstract void HandleException(HttpContext context, System.Exception exception);
        protected abstract void HandleNotFound(HttpContext context);
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            try
            {
                this._Next(context).Wait();
                if (!context.Response.HasStarted && (context.Response.StatusCode == 404))
                {
                    this.HandleNotFound(context);
                }
            }
            catch (System.Exception exception)
            {
                this.HandleException(context, exception);
            }
            return Task.CompletedTask;
        }

    }
}