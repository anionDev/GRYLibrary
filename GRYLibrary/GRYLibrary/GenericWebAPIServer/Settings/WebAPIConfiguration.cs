using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfiguration
    {
        public WebAPIConfigurationValues WebAPIConfigurationValues { get; set; }
        public Action<WebApplicationBuilder, WebAPIConfigurationValues> ConfigureBuilder { get; set; } = (builder, webAPIConfigurationValues) =>
        {

        };
        public Action<WebApplication, WebAPIConfigurationValues> ConfigureApp { get; set; } = (app, webAPIConfigurationValues) =>
        {
            string appVersionString = "v" + webAPIConfigurationValues.WebAPIConfigurationConstants.AppVersion;
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is Productive)
            {
                app.UseMiddleware<DDOSProtection>();
                app.UseMiddleware<Obfuscation>();
            }
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is not Productive)
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionManager>();
            if (!(webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is Development))
            {
                app.UseMiddleware<RequestCounter>();
            }
            app.UseMiddleware<WebApplicationFirewall>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (!(webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is Productive))
            {

                app.UseSwagger(options =>
                {
                    options.RouteTemplate = $"{webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix}/Other/Resources/{{documentName}}/{webAPIConfigurationValues.WebAPIConfigurationConstants.AppName}.api.json";
                });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"Other/Resources/{webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.SwaggerDocumentName}/{webAPIConfigurationValues.WebAPIConfigurationConstants.AppName}.api.json", webAPIConfigurationValues.WebAPIConfigurationConstants.AppName + " " + appVersionString);
                    options.RoutePrefix = webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix;
                });
            }

        };
    }
}
