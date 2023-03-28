using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Filter;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public static class GenericWebAPIServer
    {
        public static int DefaultWebAPIMainFunction<ConfigurationConstantsType, ConfigurationVariablesType>(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> configuration)
            where ConfigurationConstantsType : IWebAPIConfigurationConstants
            where ConfigurationVariablesType : IWebAPIConfigurationVariables, new()
        {
            IGeneralLogger logger = null;
            int exitCode = 1;
            try
            {
                ExecutionMode executionMode = configuration.WebAPIConfigurationValues.ExecutionMode;
                configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables = executionMode.Accept(new GetWebAPIConfigurationVariablesVisitor<ConfigurationConstantsType, ConfigurationVariablesType>(configuration));
                logger = executionMode.Accept(new GetLoggerVisitor<ConfigurationConstantsType, ConfigurationVariablesType>(configuration));
                configuration.WebAPIConfigurationValues.Logger = logger;
                IGeneralLogger.Log($"Start {configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName}", LogLevel.Information, logger);
                (WebApplication, ISet<string>) appAndUrls = CreateAPIServer(configuration);
                WebApplication app = appAndUrls.Item1;
                ISet<string> urls = appAndUrls.Item2;
                try
                {
                    IGeneralLogger.Log($"Run WebAPI-server", LogLevel.Debug, configuration.WebAPIConfigurationValues.Logger);
                    if(0 < urls.Count)
                    {
                        string urlSuffix;
                        if(HostAPIDocumentation(configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType))
                        {
                            urlSuffix = "/index.html";
                        }
                        else
                        {
                            urlSuffix = string.Empty;
                        }
                        IGeneralLogger.Log($"The API is now available under the following URLs:", LogLevel.Debug, configuration.WebAPIConfigurationValues.Logger);
                        foreach(string url in urls)
                        {
                            IGeneralLogger.Log(url + urlSuffix, LogLevel.Debug, configuration.WebAPIConfigurationValues.Logger);
                        }
                    }
                    app.Run();
                    exitCode = 0;
                }
                catch(Exception exception)
                {
                    IGeneralLogger.LogException(exception, $"Fatal error in {configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName}", configuration.WebAPIConfigurationValues.Logger);
                    exitCode = 2;
                }
                IGeneralLogger.Log($"Finished {configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName}", LogLevel.Information, configuration.WebAPIConfigurationValues.Logger);
            }
            catch(Exception exception)
            {
                string message = "Initialization-exception";
                if(logger == null)
                {
                    Console.WriteLine(message);
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(exception.StackTrace);
                }
                else
                {
                    logger.AddLogEntry(new LogItem(message, exception));
                }
                if(configuration.WebAPIConfigurationValues.RethrowInitializationExceptions)
                {
                    throw;
                }
            }
            return exitCode;
        }

        private static bool HostAPIDocumentation(GRYEnvironment environment)
        {
            return environment is not Productive;
        }

        public static ExecutionMode GetExecutionMode()
        {
            if(Assembly.GetEntryAssembly().GetName().Name == "dotnet-swagger")
            {
                return Analysis.Instance;
            }
            return RunProgram.Instance;
        }

        public static (WebApplication, ISet<string>) CreateAPIServer<ConfigurationConstantsType, ConfigurationVariablesType>(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> configuration)
            where ConfigurationConstantsType : IWebAPIConfigurationConstants
            where ConfigurationVariablesType : IWebAPIConfigurationVariables
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppName
            });
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IGeneralLogger>((serviceProvider) => configuration.WebAPIConfigurationValues.Logger);
            builder.Services.AddSingleton<IBlacklistProvider>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.BlackListProvider);
            builder.Services.AddSingleton<IDDOSProtectionSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.DDOSProtectionSettings);
            builder.Services.AddSingleton<IObfuscationSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.ObfuscationSettings);
            builder.Services.AddSingleton<IWebApplicationFirewallSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.WebApplicationFirewallSettings);
            builder.Services.AddSingleton<IExceptionManagerSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.ExceptionManagerSettings);
            builder.Services.AddSingleton<IAPIKeyValidatorSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIKeyValidatorSettings);
            builder.Services.AddSingleton<IRequestCounterSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.RequestCounterSettings);
            builder.Services.AddSingleton<IRequestLoggingSettings>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.RequestLoggingSettings);
            builder.Services.AddSingleton<IWebAPIConfigurationConstants>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants);
            builder.Services.AddSingleton<IWebAPIConfigurationVariables>((serviceProvider) => configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables);
            configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigureServices(builder.Services);
            string protocol = null;
            string localAddress = "127.0.0.1";
            string domain = localAddress;
            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.AddServerHeader = false;
                kestrelOptions.ListenAnyIP(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.Port, listenOptions =>
                {
                    if(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath == null)
                    {
                        protocol = "http";
                    }
                    else
                    {
                        protocol = "https";
                        string pfxFilePath = configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath;
                        string password = null;
                        if(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile != null)
                        {
                            password = File.ReadAllText(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile, new UTF8Encoding(false));
                        }
                        X509Certificate2 certificate = new(pfxFilePath, password);
                        if(configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType is Productive && Core.Miscellaneous.Utilities.IsSelfSIgned(certificate))
                        {
                            IGeneralLogger.Log($"The used certificate '{configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath}' is self-signed. Using self-signed certificates is not recommended in a productive environment.", LogLevel.Warning, configuration.WebAPIConfigurationValues.Logger);
                        }
                        listenOptions.UseHttps(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath, password);

                        X509Certificate2Collection collection = new X509Certificate2Collection();
                        collection.Import(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath, password, X509KeyStorageFlags.PersistKeySet);
                        List<X509Certificate2> certs = collection.ToList();
                        string dnsName = certs[0].GetNameInfo(X509NameType.DnsName, false);
                        domain = dnsName;
                    }
                });
            });
            string appVersionString = "v" + configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.AppVersion;

            builder.Services.AddControllers();
            if(HostAPIDocumentation(configuration.WebAPIConfigurationValues.WebAPIConfigurationConstants.TargetEnvironmentType))
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(swaggerOptions =>
                {
                    swaggerOptions.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                    swaggerOptions.SwaggerDoc(configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.SwaggerDocumentName, new OpenApiInfo
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
                    swaggerOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
            configuration.ConfigureBuilder(builder, configuration.WebAPIConfigurationValues);
            WebApplication app = builder.Build();
            configuration.ConfigureApp(app, configuration.WebAPIConfigurationValues);
            string generalUrl = GetURL(protocol, domain, configuration);
            string localUrl = GetURL(protocol, localAddress, configuration);
            return (app, new HashSet<string>() { generalUrl, localUrl });
        }

        private static string GetURL<ConfigurationConstantsType, ConfigurationVariablesType>(string protocol, string domain, WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> configuration)
            where ConfigurationConstantsType : IWebAPIConfigurationConstants
            where ConfigurationVariablesType : IWebAPIConfigurationVariables

        {
            return $"{protocol}://{domain}:{configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.Port}/{configuration.WebAPIConfigurationValues.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix}";
        }

        private class GetLoggerVisitor<ConfigurationConstantsType, ConfigurationVariablesType> :IExecutionModeVisitor<IGeneralLogger>
        where ConfigurationConstantsType : IWebAPIConfigurationConstants
        where ConfigurationVariablesType : IWebAPIConfigurationVariables
        {
            private readonly GRYLogConfiguration _LogConfiguration;
            public GetLoggerVisitor(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> webAPIConfiguration)
            {
                this._LogConfiguration = webAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables.ApplicationSettings.LogConfiguration;
            }
            public IGeneralLogger Handle(Analysis analysis)
            {
                return GeneralLogger.NoLog();// avoid creation of logging-entries when doing something like generate APISpecification-artifact by running "swagger tofile ..."
            }

            public IGeneralLogger Handle(RunProgram runProgram)
            {
                return GeneralLogger.Create(this._LogConfiguration);
            }
        }
        /// <summary>
        /// This visitor loads a configuration from disk if possible and if not then the initial configuration will be saved to disk and returned.
        /// </summary>
        private class GetWebAPIConfigurationVariablesVisitor<ConfigurationConstantsType, ConfigurationVariablesType> :IExecutionModeVisitor<ConfigurationVariablesType>
            where ConfigurationConstantsType : IWebAPIConfigurationConstants
            where ConfigurationVariablesType : IWebAPIConfigurationVariables, new()
        {
            private readonly MetaConfigurationSettings<ConfigurationVariablesType, IWebAPIConfigurationVariables> _MetaConfiguration;
            public GetWebAPIConfigurationVariablesVisitor(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> webAPIConfiguration)
            {
                this._MetaConfiguration = new MetaConfigurationSettings<ConfigurationVariablesType, IWebAPIConfigurationVariables>()
                {
                    ConfigurationFormat = XML.Instance,
                    File = webAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationConstants.ConfigurationFile,
                    InitialValue = webAPIConfiguration.WebAPIConfigurationValues.WebAPIConfigurationVariables
                };
            }

            public ConfigurationVariablesType Handle(Analysis analysis)
            {
                return this._MetaConfiguration.InitialValue;
            }

            public ConfigurationVariablesType Handle(RunProgram runProgram)
            {
                return MetaConfigurationManager.GetConfiguration(this._MetaConfiguration);
            }
        }
        public static string GetBaseFolderForProjectInCommonProjectStructure(GRYEnvironment environment, string programFolder)
        {
            string workspaceFolderName = "Workspace";
            if(environment is Development)
            {
                return Miscellaneous.Utilities.ResolveToFullPath($"../../{workspaceFolderName}", programFolder);
            }
            else
            {
                return $"/{workspaceFolderName}";
            }
        }

        public static GRYLogConfiguration GetDefaultLogConfiguration(string logFile, bool verbose, GRYEnvironment targetEnvironmentType)
        {
            GRYLogConfiguration result = GRYLogConfiguration.GetCommonConfiguration(logFile, verbose);
            if(targetEnvironmentType is Development)
            {
                result.GetLogTarget<GRYLibrary.Core.Log.ConcreteLogTargets.Console>().Format = GRYLogLogFormat.GRYLogFormat;
            }
            return result;
        }

        public class CreateFolderVisitor :IExecutionModeVisitor
        {
            private readonly string[] _Folder;

            public CreateFolderVisitor(params string[] folder)
            {
                this._Folder = folder;
            }

            public void Handle(Analysis analysis)
            {
                GRYLibrary.Core.Miscellaneous.Utilities.NoOperation();
            }

            public void Handle(RunProgram runProgram)
            {
                foreach(string folder in this._Folder)
                {
                    GRYLibrary.Core.Miscellaneous.Utilities.EnsureDirectoryExists(folder);
                }
            }
        }
    }
}