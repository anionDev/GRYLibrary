using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.GenericWebAPIServer.Utilities;
using GRYLibrary.Core.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfiguration
    {
        public WebAPIConfigurationValues WebAPIConfigurationValues { get; set; }
        public Action<WebApplicationBuilder, WebAPIConfigurationValues> ConfigureBuilder { get; set; } = (builder, webAPIConfigurationValues) =>
        {
            builder.Services.AddLogging(c => c.ClearProviders());
            builder.Services.AddControllers(mvcOptions =>
            {
                mvcOptions.UseGeneralRoutePrefix(webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix);
            });
        };
        public Action<WebApplication, WebAPIConfigurationValues> ConfigureApp { get; set; } = (app, webAPIConfigurationValues) =>
        {
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is Productive)
            {
                app.UseMiddleware<DDOSProtection>();
                app.UseMiddleware<Obfuscation>();
            }
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is Development)
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<RequestLoggingMiddleware>(webAPIConfigurationValues.Logger);
            app.UseMiddleware<ExceptionManager>();
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is not Development)
            {
                app.UseMiddleware<RequestCounter>();
            }
            app.UseMiddleware<WebApplicationFirewall>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is not Productive)
            {
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = $"{webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix}/Other/Resources/{{documentName}}/{webAPIConfigurationValues.WebAPIConfigurationConstants.AppName}.api.json";
                });
                app.UseSwaggerUI(options =>
                {
                    string appVersionString = "v" + webAPIConfigurationValues.WebAPIConfigurationConstants.AppVersion;
                    options.SwaggerEndpoint($"Other/Resources/{webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.SwaggerDocumentName}/{webAPIConfigurationValues.WebAPIConfigurationConstants.AppName}.api.json", webAPIConfigurationValues.WebAPIConfigurationConstants.AppName + " " + appVersionString);
                    options.RoutePrefix = webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix;
                });
            }
        };
    }
}
