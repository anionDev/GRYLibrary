using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents a configuration-container with all runtime-information which are required to run a WebAPI. 
    /// </summary>
    public class WebAPIConfiguration<ConfigurationConstantsType, PersistentConfigurationTypes>
        where ConfigurationConstantsType : IWebAPIConfigurationConstants
        where PersistentConfigurationTypes : WebAPIConfigurationVariables<PersistentConfigurationTypes>,new()
    {
        public WebAPIConfiguration(){
            }
        public IGeneralLogger Logger { get; set; }
        public ConfigurationConstantsType WebAPIConfigurationConstants { get; set; }
        public PersistentConfigurationTypes WebAPIConfigurationVariables { get; set; }
        public string BasePath { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public bool RethrowInitializationExceptions { get; set; }
        public string[] CommandlineArguments { get; set; }
        public Action<WebApplicationBuilder, WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType>> ConfigureBuilder { get; set; } = (builder, webAPIConfigurationValues) =>
        {
            builder.Services.AddLogging(c => c.ClearProviders());
            builder.Services.AddControllers(mvcOptions => mvcOptions.UseGeneralRoutePrefix(webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix));
        };
        public Action<WebApplication, WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType>> ConfigureApp { get; set; } = (app, webAPIConfigurationValues) =>
        {
            #region General Threat-Protection
            if(webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is Productive)
            {
                app.UseMiddleware<DDOSProtection>();
                app.UseMiddleware<BlackList>();
                app.UseMiddleware<Obfuscation>();
            }
            if(webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is not Development)
            {
                app.UseMiddleware<ExceptionManager>();
            }
            #endregion

            #region Diagnosis
            app.UseMiddleware<RequestLoggingMiddleware>();
            if(webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is Development)
            {
                app.UseDeveloperExceptionPage();
            }
            #endregion

            #region Bussiness-implementation of access-restriction
            app.UseMiddleware<WebApplicationFirewall>();
            app.UseMiddleware<APIKeyValidator>();
            if(webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is not Development)
            {
                app.UseMiddleware<RequestCounter>();
            }
            #endregion

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            if(webAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is not Productive)
            {
                app.UseSwagger(options => options.RouteTemplate = $"{webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix}/Other/Resources/{{documentName}}/{webAPIConfigurationValues.WebAPIConfigurationConstants.AppName}.api.json");
                app.UseSwaggerUI(options =>
                {
                    string appVersionString = "v" + webAPIConfigurationValues.WebAPIConfigurationConstants.AppVersion;
                    options.SwaggerEndpoint($"Other/Resources/{webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.SwaggerDocumentName}/{webAPIConfigurationValues.WebAPIConfigurationConstants.AppName}.api.json", webAPIConfigurationValues.WebAPIConfigurationConstants.AppName + " " + appVersionString);
                    options.RoutePrefix = webAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix;
                });
            }
        };
        public Action PreRun { get; set; }
        public Action PostRun { get; set; } 
    }
}