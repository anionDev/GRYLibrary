using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a middleware which removes some not required information of responses for security purposes.
    /// </summary>
    public class Obfuscation : AbstractMiddleware
    {
        private readonly IObfuscationSettings _ObfuscationSettings;
        private readonly IWebAPIConfigurationVariables _WebAPIConfigurationVariables;
        private readonly IWebAPIConfigurationConstants _WebAPIConfigurationConstants;
        /// <inheritdoc/>
        public Obfuscation(RequestDelegate next, IObfuscationSettings obfuscationSettings, IWebAPIConfigurationVariables webAPIConfigurationVariables, IWebAPIConfigurationConstants webAPIConfigurationConstants) : base(next)
        {
            this._ObfuscationSettings = obfuscationSettings;
            this._WebAPIConfigurationVariables = webAPIConfigurationVariables;
            this._WebAPIConfigurationConstants = webAPIConfigurationConstants;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            if (this._WebAPIConfigurationConstants.TargetEnvironmentType is Productive)
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
