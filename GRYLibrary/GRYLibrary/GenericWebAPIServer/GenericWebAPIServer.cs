using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public static class GenericWebAPIServer
    {
        public static int DefaultWebAPIMainFunction(WebAPIConfiguration initialionWebAPIConfiguration)
        {
            int exitCode;
            IGeneralLogger logger;
            WebAPIConfigurationVariables webAPIConfigurationVariables = Miscellaneous.Utilities.CreateOrLoadLoadJSONConfigurationFile(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigurationFileName, initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables);
            if (Assembly.GetEntryAssembly().GetName().Name == "dotnet-swagger")
            {
                logger = GeneralLogger.NoLog();// avoid creation of logging-entries when generating APISpecification-artifact by running "swagger tofile ..."
            }
            else
            {
                logger = GeneralLogger.Create(initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName, initialionWebAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables.ApplicationSettings.LogFolder);
            }
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
            builder.Services.AddSingleton<IGeneralLogger>((serviceProvider) => configuration.WebAPIConfigurationValues.Logger);
            builder.Services.AddSingleton<IBlacklistProvider>((serviceProvider) => configuration.WebAPIConfigurationValues.BlackListProvider);
            builder.Services.AddSingleton<IDDOSProtectionSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.DDOSProtectionSettings);
            builder.Services.AddSingleton<IObfuscationSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.ObfuscationSettings);
            builder.Services.AddSingleton<IWebApplicationFirewallSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebApplicationFirewallSettings);
            builder.Services.AddSingleton<IExceptionManagerSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.ExceptionManagerSettings);
            builder.Services.AddSingleton<IRequestCounterSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.RequestCounterSettings);
            builder.Services.AddSingleton<IWebAPIConfigurationConstants>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants);
            builder.Services.AddSingleton<IWebAPIConfigurationVariables>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables);
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ListenAnyIP(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.Port, listenOptions =>
                {
                    if (configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath != null)
                    {
                        string pfxFilePath = configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile;
                        string password = null;
                        if (configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile != null)
                        {
                            password = File.ReadAllText(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath, new UTF8Encoding(false));
                        }
                        X509Certificate2 certificate = new(pfxFilePath, password);
                        if (configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.GetTargetEnvironmentType() is Productive && Core.Miscellaneous.Utilities.IsSelfSIgned(certificate))
                        {
                            IGeneralLogger.Log($"The used certificate '{configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath}' is self-signed. Using self-signed certificates is not recommended in a productive environment.", LogLevel.Warning, configuration.WebAPIConfigurationValues.Logger);
                        }
                        listenOptions.UseHttps(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath, "password");
                    }
                });
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
                    string xmlFilename = $"{configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName}.xml";
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
