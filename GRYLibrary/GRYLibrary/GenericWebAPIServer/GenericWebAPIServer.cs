﻿using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
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
    /// <summary>
    /// Runs a WebAPI and blocks the thread until the webserveris down.
    /// </summary>
    public static class GenericWebAPIServer
    {
        public static int WebAPIRunnter<ConfigurationConstantsType, ConfigurationVariablesType>(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> configuration, Action preRun, Action postRun)
            where ConfigurationConstantsType : IWebAPIConfigurationConstants
            where ConfigurationVariablesType : IWebAPIConfigurationVariables, new()
        {
            int exitCode = 2;
            IGeneralLogger logger = configuration.Logger;
            ExecutionMode executionMode = configuration.ExecutionMode;
            (WebApplication, ISet<string>) appAndUrls = default;
            bool startApplication = true;
            try
            {
                HashSet<Type> knownTypes = new HashSet<Type> { typeof(ConfigurationVariablesType) };
                configuration.WebAPIConfigurationVariables = executionMode.Accept(new GetWebAPIConfigurationVariablesVisitor<ConfigurationConstantsType, ConfigurationVariablesType>(configuration, knownTypes));
                configuration.Logger = logger;
                logger.Log($"Start {configuration.WebAPIConfigurationConstants.AppName}", LogLevel.Information);
                appAndUrls = CreateAPIServer(configuration);
            }
            catch(Exception exception)
            {
                startApplication = false;
                string message = "Initialization-exception";
                logger.AddLogEntry(new LogItem(message, exception));
                if(configuration.RethrowInitializationExceptions)
                {
                    throw;
                }
                else
                {
                    exitCode = 4;
                }
            }

            if(startApplication && !configuration.WebAPIConfigurationVariables.GeneralApplicationSettings.Enabled)
            {
                startApplication = false;
                logger.Log($"Application is disabled.", LogLevel.Information);
            }

            if(startApplication)
            {
                WebApplication app = appAndUrls.Item1;
                ISet<string> urls = appAndUrls.Item2;
                try
                {
                    if(0 < urls.Count)
                    {
                        string urlSuffix;
                        if(HostAPIDocumentation(configuration.WebAPIConfigurationConstants.TargetEnvironmentType))
                        {
                            urlSuffix = "/index.html";
                        }
                        else
                        {
                            urlSuffix = string.Empty;
                        }
                        configuration.Logger.Log($"The API is available under the following URLs:", LogLevel.Debug);
                        foreach(string url in urls)
                        {
                            configuration.Logger.Log(url + urlSuffix, LogLevel.Debug);
                        }
                    }

                    logger.Log($"Run Pre-action", LogLevel.Debug);
                    preRun();

                    logger.Log($"Run WebAPI-server", LogLevel.Debug);
                    app.Run();

                    logger.Log($"Run Post-action", LogLevel.Debug);
                    postRun();

                    exitCode = 0;
                }
                catch(Exception exception)
                {
                    configuration.Logger.LogException(exception, $"Fatal error in {configuration.WebAPIConfigurationConstants.AppName}.");
                    exitCode = 3;
                }
                configuration.Logger.Log($"Finished {configuration.WebAPIConfigurationConstants.AppName}", LogLevel.Information);
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
                ApplicationName = configuration.WebAPIConfigurationConstants.AppName
            });
            builder.Services.AddControllers();
            builder.Services.AddSingleton((serviceProvider) => configuration.Logger);
            builder.Services.AddSingleton<IBlacklistProvider>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.BlackListProvider);
            builder.Services.AddSingleton<IDDOSProtectionSettings>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.DDOSProtectionSettings);
            builder.Services.AddSingleton<IObfuscationSettings>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.ObfuscationSettings);
            builder.Services.AddSingleton<IWebApplicationFirewallSettings>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.WebApplicationFirewallSettings);
            builder.Services.AddSingleton<IExceptionManagerSettings>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.ExceptionManagerSettings);
            builder.Services.AddSingleton<IAPIKeyValidatorSettings>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.APIKeyValidatorSettings);
            builder.Services.AddSingleton<IRequestCounterSettings>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.RequestCounterSettings);
            builder.Services.AddSingleton<IRequestLoggingSettings>((serviceProvider) => configuration.WebAPIConfigurationVariables.WebServerSettings.RequestLoggingSettings);
            builder.Services.AddSingleton<IWebAPIConfigurationConstants>((serviceProvider) => configuration.WebAPIConfigurationConstants);
            builder.Services.AddSingleton<IWebAPIConfigurationVariables>((serviceProvider) => configuration.WebAPIConfigurationVariables);
            configuration.WebAPIConfigurationConstants.ConfigureServices(builder.Services);
            string protocol = null;
            string localAddress = "127.0.0.1";
            string domain = localAddress;
            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.AddServerHeader = false;
                kestrelOptions.ListenAnyIP(configuration.WebAPIConfigurationVariables.WebServerSettings.Port, listenOptions =>
                {
                    if(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath == null)
                    {
                        protocol = "http";
                    }
                    else
                    {
                        protocol = "https";
                        string pfxFilePath = configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath;
                        string password = null;
                        if(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile != null)
                        {
                            password = File.ReadAllText(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile, new UTF8Encoding(false));
                        }
                        X509Certificate2 certificate = new(pfxFilePath, password);
                        if(configuration.WebAPIConfigurationConstants.TargetEnvironmentType is Productive && Miscellaneous.Utilities.IsSelfSIgned(certificate))
                        {
                            configuration.Logger.Log($"The used certificate '{configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath}' is self-signed. Using self-signed certificates is not recommended in a productive environment.", LogLevel.Warning);
                        }
                        listenOptions.UseHttps(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath, password);

                        X509Certificate2Collection collection = new X509Certificate2Collection();
                        collection.Import(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath, password, X509KeyStorageFlags.PersistKeySet);
                        List<X509Certificate2> certs = collection.ToList();
                        string dnsName = certs[0].GetNameInfo(X509NameType.DnsName, false);
                        domain = dnsName;
                    }
                });
            });
            string appVersionString = "v" + configuration.WebAPIConfigurationConstants.AppVersion;

            builder.Services.AddControllers();
            if(HostAPIDocumentation(configuration.WebAPIConfigurationConstants.TargetEnvironmentType))
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(swaggerOptions =>
                {
                    swaggerOptions.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                    swaggerOptions.SwaggerDoc(configuration.WebAPIConfigurationVariables.WebServerSettings.SwaggerDocumentName, new OpenApiInfo
                    {
                        Version = appVersionString,
                        Title = configuration.WebAPIConfigurationConstants.AppName + " API",
                        Description = configuration.WebAPIConfigurationVariables.GeneralApplicationSettings.AppDescription,
                        TermsOfService = new Uri(configuration.WebAPIConfigurationVariables.GeneralApplicationSettings.TermsOfServiceURL),
                        Contact = new OpenApiContact
                        {
                            Name = "Contact",
                            Url = new Uri(configuration.WebAPIConfigurationVariables.GeneralApplicationSettings.ContactURL)
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri(configuration.WebAPIConfigurationVariables.GeneralApplicationSettings.LicenseURL)
                        }
                    });
                    string xmlFilename = $"{configuration.WebAPIConfigurationConstants.AppName}.xml";
                    swaggerOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
            configuration.ConfigureBuilder(builder, configuration);
            WebApplication app = builder.Build();
            configuration.ConfigureApp(app, configuration);
            string generalUrl = GetURL(protocol, domain, configuration);
            string localUrl = GetURL(protocol, localAddress, configuration);
            return (app, new HashSet<string>() { generalUrl, localUrl });
        }

        private static string GetURL<ConfigurationConstantsType, ConfigurationVariablesType>(string protocol, string domain, WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> configuration)
            where ConfigurationConstantsType : IWebAPIConfigurationConstants
            where ConfigurationVariablesType : IWebAPIConfigurationVariables

        {
            return $"{protocol}://{domain}:{configuration.WebAPIConfigurationVariables.WebServerSettings.Port}/{configuration.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix}";
        }

        public class GetLoggerVisitor :IExecutionModeVisitor<IGeneralLogger>
        {
            private readonly GRYLogConfiguration _LogConfiguration;
            public GetLoggerVisitor(GRYLogConfiguration logConfiguration)
            {
                this._LogConfiguration = logConfiguration;
            }
            public IGeneralLogger Handle(Analysis analysis)
            {
                return GeneralLogger.NoLog();// avoid creation of logging-entries when doing something like generate APISpecification-artifact by running "swagger tofile ..."
            }

            public IGeneralLogger Handle(RunProgram runProgram)
            {
                return GeneralLogger.CreateUsingGRYLog(this._LogConfiguration);
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
            private readonly ISet<Type> _KnownTypes;
            public GetWebAPIConfigurationVariablesVisitor(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> webAPIConfiguration, ISet<Type> knownTypes)
            {
                this._MetaConfiguration = new MetaConfigurationSettings<ConfigurationVariablesType, IWebAPIConfigurationVariables>()
                {
                    ConfigurationFormat = XML.Instance,
                    File = webAPIConfiguration.WebAPIConfigurationConstants.ConfigurationFile,
                    InitialValue = webAPIConfiguration.WebAPIConfigurationVariables
                };
                this._KnownTypes = knownTypes;
            }

            public ConfigurationVariablesType Handle(Analysis analysis)
            {
                return this._MetaConfiguration.InitialValue;
            }

            public ConfigurationVariablesType Handle(RunProgram runProgram)
            {
                return MetaConfigurationManager.GetConfiguration(this._MetaConfiguration, this._KnownTypes);
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
                result.GetLogTarget<Log.ConcreteLogTargets.Console>().Format = GRYLogLogFormat.GRYLogFormat;
            }
            return result;
        }
    }
}