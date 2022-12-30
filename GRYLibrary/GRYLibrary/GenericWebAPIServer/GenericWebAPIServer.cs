using GRYLibrary.Core.GenericWebAPIServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class GenericWebAPIServer
    {

        public static int DefaultWebAPIMainFunction(WebAPIConfiguration initialionWebAPIConfiguration)
        {
            Encoding configurationFileEncoding = new UTF8Encoding(false);
            WebAPIConfigurationVariables webAPIConfigurationVariables = null;
            if (File.Exists(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigurationFileName))
            {
                IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigurationFileName).Build();
                webAPIConfigurationVariables = configurationRoot.GetRequiredSection(nameof(WebAPIConfigurationVariables)).Get<WebAPIConfigurationVariables>();
            }
            else
            {
                webAPIConfigurationVariables = initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables;
                dynamic configurationFileObject = new
                {
                    WebAPIConfigurationVariables = webAPIConfigurationVariables
                };
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(configurationFileObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigurationFileName, serialized, configurationFileEncoding);
            }
            RunAPIServer(new WebAPIConfiguration()
            {
                ConfigureApp = initialionWebAPIConfiguration.ConfigureApp,
                ConfigureBuilder= initialionWebAPIConfiguration.ConfigureBuilder,
                WebAPIConfigurationValues=new WebAPIConfigurationValues()
                {
                    WebAPIConfigurationConstants = initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants,
                    WebAPIConfigurationVariables = webAPIConfigurationVariables,
                }
            });
            return 0;
        }
        public static void RunAPIServer(WebAPIConfiguration configuration)
        {
            bool isDevelopmentEnvironmentType = configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType == "Development";
            bool isQualityCheckEnvironmentType = configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType == "QualityCheck";
            bool isProductiveEnvironmentType = configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType == "Productive";
            //TODO assert app.Environment.IsDevelopment()==isDevelopmentEnvironmentType
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
            if (!isProductiveEnvironmentType)
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
            configuration.ConfigureBuilder(builder,configuration.WebAPIConfigurationValues);
            WebApplication app = builder.Build();
           configuration.ConfigureApp(app, configuration.WebAPIConfigurationValues);
            app.Run();
        }
    }
}
