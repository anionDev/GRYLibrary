using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfiguration
    {
        public WebAPIConfigurationConstants WebAPIConfigurationConstants { get; set; }
        public WebAPIConfigurationVariables WebAPIConfigurationVariables { get; set; }
        public Action<WebApplicationBuilder, WebAPIConfigurationConstants, WebAPIConfigurationVariables> Configure { get; set; } = (builder, webAPIConfigurationConstants, webAPIConfigurationVariables) =>
        {

            if (webAPIConfigurationConstants.GetTargetEnvironmentType() is Productive)
            {
                app.UseMiddleware<DDOSProtection>();
                app.UseMiddleware<Obfuscation>();
            }
            if (webAPIConfigurationConstants.GetTargetEnvironmentType() is not Productive)
            {
                app.UseDeveloperExceptionPage();
            }
            if (CurrentSettings.WebServerSettings.BasePath != null)
            {
                app.UsePathBase(CurrentSettings.WebServerSettings.BasePath);
            }
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionManager>();
            if (!webAPIConfigurationConstants.GetTargetEnvironmentType() is Development)
            {
                app.UseMiddleware<RequestCounter>();
            }
            app.UseMiddleware<WebApplicationFirewall>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        };
    }
}
