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
    public class New
    {

        private static int DefaultWebAPIMainFunction(WebAPIConfiguration initialionWebAPIConfiguration)
        {
            Encoding configurationFileEncoding = new UTF8Encoding(false);
            WebAPIConfigurationVariables webAPIConfigurationVariables = null;
            if (File.Exists(initialionWebAPIConfiguration.WebAPIConfigurationConstants.ConfigurationFileName))
            {
                IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile(initialionWebAPIConfiguration.WebAPIConfigurationConstants.ConfigurationFileName).Build();
                webAPIConfigurationVariables = configurationRoot.GetRequiredSection(nameof(WebAPIConfigurationVariables)).Get<WebAPIConfigurationVariables>();
            }
            else
            {
                webAPIConfigurationVariables = initialionWebAPIConfiguration.WebAPIConfigurationVariables;
                dynamic configurationFileObject = new
                {
                    WebAPIConfigurationVariables = webAPIConfigurationVariables
                };
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(configurationFileObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(initialionWebAPIConfiguration.WebAPIConfigurationConstants.ConfigurationFileName, serialized, configurationFileEncoding);
            }
            RunAPIServer(new WebAPIConfiguration()
            {
                WebAPIConfigurationConstants = initialionWebAPIConfiguration.WebAPIConfigurationConstants,
                WebAPIConfigurationVariables = webAPIConfigurationVariables,
            });
            return 0;
        }
        private static void RunAPIServer(WebAPIConfiguration configuration)
        {
            bool isDevelopmentEnvironmentType = configuration.WebAPIConfigurationConstants.TargetEnvironmentType == "Development";
            bool isQualityCheckEnvironmentType = configuration.WebAPIConfigurationConstants.TargetEnvironmentType == "QualityCheck";
            bool isProductiveEnvironmentType = configuration.WebAPIConfigurationConstants.TargetEnvironmentType == "Productive";
            //TODO assert app.Environment.IsDevelopment()==isDevelopmentEnvironmentType
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = configuration.WebAPIConfigurationConstants.AppName
            });
            builder.Services.AddControllers();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ListenAnyIP(configuration.WebAPIConfigurationVariables.Port);
            });
            string appVersionString = "v" + configuration.WebAPIConfigurationConstants.AppVersion;
            string swaggerDocumentName = configuration.WebAPIConfigurationConstants.SwaggerDocumentName;

            if (!isProductiveEnvironmentType)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(swaggerDocumentName, new OpenApiInfo
                    {
                        Version = appVersionString,
                        Title = configuration.WebAPIConfigurationConstants.AppName + " API",
                        Description = configuration.WebAPIConfigurationVariables.AppDescription,
                        TermsOfService = new Uri(configuration.WebAPIConfigurationVariables.TermsOfServiceURL),
                        Contact = new OpenApiContact
                        {
                            Name = "Contact",
                            Url = new Uri(configuration.WebAPIConfigurationVariables.ContactURL)
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri(configuration.WebAPIConfigurationVariables.LicenseURL)
                        }
                    });
                    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
            WebApplication app = builder.Build();
            if (!isProductiveEnvironmentType)
            {
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = $"{configuration.WebAPIConfigurationVariables.APIRoutePrefix}/Other/Resources/{{documentName}}/{configuration.WebAPIConfigurationConstants.AppName}.api.json";
                });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"Other/Resources/{swaggerDocumentName}/{configuration.WebAPIConfigurationConstants.AppName}.api.json", configuration.WebAPIConfigurationConstants.AppName + " " + appVersionString);
                    options.RoutePrefix = configuration.WebAPIConfigurationVariables.APIRoutePrefix;
                });
            }
            app.MapControllers();
            app.Run();
        }
    }
    public class WebAPIConfiguration
    {
        public WebAPIConfigurationConstants WebAPIConfigurationConstants { get; set; }
        public WebAPIConfigurationVariables WebAPIConfigurationVariables { get; set; }
    }
    public class WebAPIConfigurationConstants
    {
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string TargetEnvironmentType { get; set; }
        public string SwaggerDocumentName { get; set; } = "APISpecification";
        public string ConfigurationFileName { get; set; } = "APIServerAppSettings.json";
    }
    public class WebAPIConfigurationVariables
    {
        public string TermsOfServiceURL { get; set; }
        public string ContactURL { get; set; }
        public string LicenseURL { get; set; }
        public string AppDescription { get; set; }
        public ushort Port { get; set; } = 4422;
        public string APIRoutePrefix { get; set; } = "API";
    }
}
