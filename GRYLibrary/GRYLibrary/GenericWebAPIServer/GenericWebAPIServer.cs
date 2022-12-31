using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class GenericWebAPIServer
    {

        public static int DefaultWebAPIMainFunction(WebAPIConfiguration initialionWebAPIConfiguration)
        {
            WebAPIConfigurationVariables webAPIConfigurationVariables = LoadConfiguration(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigurationFileName, initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables);
            RunAPIServer(new WebAPIConfiguration()
            {
                ConfigureApp = initialionWebAPIConfiguration.ConfigureApp,
                ConfigureBuilder = initialionWebAPIConfiguration.ConfigureBuilder,
                WebAPIConfigurationValues = new WebAPIConfigurationValues()
                {
                    WebAPIConfigurationConstants = initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants,
                    WebAPIConfigurationVariables = webAPIConfigurationVariables,
                }
            });
            return 0;
        }

        private static T LoadConfiguration<T>(string configurationFile, T initialValue)
        {
            Encoding configurationFileEncoding = new UTF8Encoding(false);
            T configuration;
            if (File.Exists(configurationFile))
            {
                IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile(configurationFile).Build();
                configuration = configurationRoot.GetRequiredSection(typeof(T).Name).Get<T>();
            }
            else
            {
                configuration = initialValue;
                dynamic expando = new ExpandoObject();
                ((IDictionary<String, object>)expando)[typeof(T).Name] = configuration;
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(expando, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(configurationFile, serialized, configurationFileEncoding);
            }

            return configuration;
        }

        public static void RunAPIServer(WebAPIConfiguration configuration)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName
            });
            builder.Services.AddControllers();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ListenAnyIP(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.Port);
            });
            string appVersionString = "v" + configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppVersion;

            builder.Services.AddControllers();
            if (!(configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is Productive))
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
            app.Run();
        }
    }
}
