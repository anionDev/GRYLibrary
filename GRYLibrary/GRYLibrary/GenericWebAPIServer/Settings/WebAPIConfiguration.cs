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
    public class WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType>
        where ConfigurationConstantsType : IWebAPIConfigurationConstants
        where ConfigurationVariablesType : IWebAPIConfigurationVariables
    {
        public WebAPIConfigurationValues<ConfigurationConstantsType, ConfigurationVariablesType> WebAPIConfigurationValues { get; set; }
        public Action<WebApplicationBuilder, WebAPIConfigurationValues<ConfigurationConstantsType, ConfigurationVariablesType>> ConfigureBuilder { get; set; } = (builder, webAPIConfigurationValues) =>
        {
            builder.Services.AddLogging(c => c.ClearProviders());
            builder.Services.AddControllers(mvcOptions =>
            {
                mvcOptions.UseGeneralRoutePrefix(webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix);
            });
        };
        public Action<WebApplication, WebAPIConfigurationValues<ConfigurationConstantsType, ConfigurationVariablesType>> ConfigureApp { get; set; } = (app, webAPIConfigurationValues) =>
        {
            #region General Threat-Protection
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is Productive)
            {
                app.UseMiddleware<DDOSProtection>();
                app.UseMiddleware<BlackList>();
                app.UseMiddleware<Obfuscation>();
            }
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is not Development)
            {
                app.UseMiddleware<ExceptionManager>();
            }
            #endregion

            #region Diagnosis
            app.UseMiddleware<RequestLoggingMiddleware>();
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is Development)
            {
                app.UseDeveloperExceptionPage();
            }
            #endregion

            #region Bussiness-implementation of access-restriction
            app.UseMiddleware<WebApplicationFirewall>();
            app.UseMiddleware<APIKeyValidator>();
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is not Development)
            {
                app.UseMiddleware<RequestCounter>();
            }
            #endregion

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is not Productive)
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
