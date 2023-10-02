using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Settings;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Obfuscation
{
    /// <summary>
    /// Represents a middleware which removes some not required information of responses for security purposes.
    /// </summary>
    public class ObfuscationMiddleware : AbstractMiddleware
    {
        private readonly IObfuscationConfiguration _ObfuscationSettings;
        private readonly IApplicationConstants _AppConstants;
        /// <inheritdoc/>
        public ObfuscationMiddleware(RequestDelegate next, IObfuscationConfiguration obfuscationSettings, IApplicationConstants appConstants) : base(next)
        {
            this._ObfuscationSettings = obfuscationSettings;
            this._AppConstants = appConstants;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            if (this._AppConstants.Environment is Productive)
            {
                bool clearResponseBody;
                int responseStatusCode;
                if (context.Response.StatusCode == 401)
                {
                    clearResponseBody = true;
                    responseStatusCode = context.Response.StatusCode;
                }
                else if (context.Response.StatusCode == 403)
                {
                    clearResponseBody = true;
                    responseStatusCode = context.Response.StatusCode;
                }
                else if (context.Response.StatusCode % 100 == 2)
                {
                    responseStatusCode = 200;
                    clearResponseBody = false;
                }
                else
                {
                    clearResponseBody = true;
                    responseStatusCode = 400;
                }
                context.Response.StatusCode = responseStatusCode; //TODO check why this does not work properly
                if (clearResponseBody)
                {
                    //TODO
                }
            }
            return this._Next(context);
        }
    }
}