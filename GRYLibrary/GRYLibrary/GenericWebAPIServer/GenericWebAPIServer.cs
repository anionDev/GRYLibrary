using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public static class GenericWebAPIServer
    {
        public static int DefaultWebAPIMainFunction(WebAPIConfiguration initialionWebAPIConfiguration)
        {
            int exitCode;
            WebAPIConfigurationVariables webAPIConfigurationVariables = Miscellaneous.Utilities.CreateOrLoadLoadJSONConfigurationFile(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigurationFileName, initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables);
            IGeneralLogger logger = GRYLogLogger.Create(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName, initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables.ApplicationSettings.LogFolder);
            initialionWebAPIConfiguration.WebAPIConfigurationValues.Logger = logger;
            var webAPIConfiguration = new WebAPIConfiguration()
            {
                ConfigureApp = initialionWebAPIConfiguration.ConfigureApp,
                ConfigureBuilder = initialionWebAPIConfiguration.ConfigureBuilder,
                WebAPIConfigurationValues = new WebAPIConfigurationValues()
                {
                    WebAPIConfigurationConstants = initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants,
                    WebAPIConfigurationVariables = webAPIConfigurationVariables,
                    Logger = logger,
                },
            };
            IGeneralLogger.Log($"Start {initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName}", LogLevel.Information, logger);
            try
            {
                RunAPIServer(webAPIConfiguration);
                exitCode = 0;
            }
            catch (Exception exception)
            {
                IGeneralLogger.LogException(exception, $"Fatal error in {initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName}", initialionWebAPIConfiguration.WebAPIConfigurationValues.Logger);
                exitCode = 1;
            }
            IGeneralLogger.Log($"Finished {initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName}", LogLevel.Information, initialionWebAPIConfiguration.WebAPIConfigurationValues.Logger);
            return exitCode;
        }

        public static void RunAPIServer(WebAPIConfiguration configuration)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName
            });
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IGeneralLogger>((serviceProvider)=> configuration.WebAPIConfigurationValues.Logger);
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ListenAnyIP(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.Port);
            });
            string appVersionString = "v" + configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppVersion;

            builder.Services.AddControllers();
            if (configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is not Productive)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.SwaggerDocumentName, new OpenApiInfo
                    {
                        Version = appVersionString,
                        Title = configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName + " API",
                        Description = configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.ApplicationSettings.AppDescription,
                        TermsOfService = new Uri(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.ApplicationSettings.TermsOfServiceURL),
                        Contact = new OpenApiContact
                        {
                            Name = "Contact",
                            Url = new Uri(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.ApplicationSettings.ContactURL)
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.ApplicationSettings.LicenseURL)
                        }
                    });
                    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
            configuration.ConfigureBuilder(builder, configuration.WebAPIConfigurationValues);
            WebApplication app = builder.Build();
            configuration.ConfigureApp(app, configuration.WebAPIConfigurationValues);
            IGeneralLogger.Log($"Start WebAPI-server", LogLevel.Information, configuration.WebAPIConfigurationValues.Logger);
            app.Run();
        }
    }
}
