using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Maint
{
    public abstract class MaintenanceSiteMiddleware : AbstractMiddleware
    {
        private readonly IMaintenanceSiteConfiguration _MaintenanceSiteConfiguration;
        protected abstract (string ContentType, string bodyContent) GetMaintenanceSite(HttpContext context);
        //Returns true if and only if the context is allowed to be loaded even in maintenance-mode.
        protected MaintenanceSiteMiddleware(RequestDelegate next, IMaintenanceSiteConfiguration maintenanceSiteConfiguration) : base(next)
        {
            this._MaintenanceSiteConfiguration = maintenanceSiteConfiguration;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            if (this._MaintenanceSiteConfiguration.MaintenanceModeEnabled && !this.IsException(context))
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                (string ContentType, string bodyContent) = this.GetMaintenanceSite(context);
                context.Response.ContentType = ContentType;
                context.Response.WriteAsync(bodyContent).Wait();
            }
            else
            {
                this._Next(context).Wait();
            }
            return Task.CompletedTask;
        }
        protected virtual bool IsException(HttpContext context)
        {
            return false;
        }
    }
}