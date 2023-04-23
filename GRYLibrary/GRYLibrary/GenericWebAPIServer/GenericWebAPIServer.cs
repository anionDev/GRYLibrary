using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Filter;
using GRYLibrary.Core.GenericWebAPIServer.Services;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public static int WebAPIRunner<ConfigurationConstantsType, ConfigurationVariablesType>(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> configuration, IInjectableSettings injectableSettings)
            where ConfigurationConstantsType : IWebAPIConfigurationConstants
            where ConfigurationVariablesType : IWebAPIConfigurationVariables, new()
        {
            int exitCode = 2;
            IGeneralLogger logger = configuration.Logger;
            ExecutionMode executionMode = configuration.ExecutionMode;
            (WebApplication, string) appAndUrl = default;
            bool startApplication = true;
            try
            {
                HashSet<Type> knownTypes = new HashSet<Type> { typeof(ConfigurationVariablesType) };
                configuration.WebAPIConfigurationVariables = executionMode.Accept(new GetWebAPIConfigurationVariablesVisitor<ConfigurationConstantsType, ConfigurationVariablesType>(configuration, knownTypes));
                configuration.Logger = logger;
                logger.Log($"Start {configuration.WebAPIConfigurationConstants.AppName}", LogLevel.Information);
                appAndUrl = CreateAPIServer(configuration, injectableSettings);
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
                WebApplication app = appAndUrl.Item1;
                string url = appAndUrl.Item2;
                try
                {
                    string urlSuffix;
                    if(HostAPIDocumentation(configuration.WebAPIConfigurationConstants.TargetEnvironmentType, configuration.WebAPIConfigurationVariables.WebServerSettings.HostAPISpecificationForInNonDevelopmentEnvironment, configuration.ExecutionMode))
                    {
                        urlSuffix = "/index.html";
                    }
                    else
                    {
                        urlSuffix = string.Empty;
                    }
                    configuration.Logger.Log($"The API is available under the following URL:", LogLevel.Debug);
                    configuration.Logger.Log(url + urlSuffix, LogLevel.Debug);

                    logger.Log($"Run Pre-action", LogLevel.Debug);
                    configuration.PreRun();

                    logger.Log($"Run WebAPI-server", LogLevel.Debug);
                    app.Run();

                    logger.Log($"Run Post-action", LogLevel.Debug);
                    configuration.PostRun();

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

        private static bool HostAPIDocumentation(GRYEnvironment environment, bool hostAPISpecificationForInNonDevelopmentEnvironment, ExecutionMode executionMode)
        {
            return executionMode.Accept(new GetHostAPIDocumentationVisitor(environment, hostAPISpecificationForInNonDevelopmentEnvironment));
        }
        private class GetHostAPIDocumentationVisitor :IExecutionModeVisitor<bool>
        {
            private readonly GRYEnvironment _Environment;
            private readonly bool _HostAPISpecificationForInNonDevelopmentEnvironment;

            public GetHostAPIDocumentationVisitor(GRYEnvironment environment, bool hostAPISpecificationForInNonDevelopmentEnvironment)
            {
                this._Environment = environment;
                this._HostAPISpecificationForInNonDevelopmentEnvironment = hostAPISpecificationForInNonDevelopmentEnvironment;
            }

            public bool Handle(Analysis analysis)
            {
                return true;
            }

            public bool Handle(RunProgram runProgram)
            {
                if(this._Environment is Development)
                {
                    return true;
                }
                else
                {
                    return this._HostAPISpecificationForInNonDevelopmentEnvironment;
                }
            }
        }

        public static ExecutionMode GetExecutionMode()
        {
            if(Assembly.GetEntryAssembly().GetName().Name == "dotnet-swagger")
            {
                return Analysis.Instance;
            }
            return RunProgram.Instance;
        }

        public static (WebApplication, string) CreateAPIServer<ConfigurationConstantsType, ConfigurationVariablesType>(WebAPIConfiguration<ConfigurationConstantsType, ConfigurationVariablesType> configuration, IInjectableSettings injectableSettings)
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
            // builder.Services.AddSingleton<>();//TODO add settings from configfile. therefor it is probably requried to enforce developer to use CustomWebAPIConfigurationVariables<CustomConfigType> for the settings. then the settings are less configurable/configurable in another way. then some things can probably be simplified/refactored.
            configuration.WebAPIConfigurationConstants.ConfigureServices(builder.Services);
            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.AddServerHeader = false;
                kestrelOptions.ListenAnyIP(configuration.WebAPIConfigurationVariables.WebServerSettings.Port, listenOptions =>
                {
                    if(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath != null)
                    {
                        string pfxFilePath = configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath.GetPath(configuration.BasePath);
                        string password = null;
                        if(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile != null)
                        {
                            password = File.ReadAllText(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePasswordFile.GetPath(configuration.BasePath), new UTF8Encoding(false));
                        }
                        X509Certificate2 certificate = new(pfxFilePath, password);
                        if(configuration.WebAPIConfigurationConstants.TargetEnvironmentType is Productive && Miscellaneous.Utilities.IsSelfSIgned(certificate))
                        {
                            configuration.Logger.Log($"The used certificate '{configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath}' is self-signed. Using self-signed certificates is not recommended in a productive environment.", LogLevel.Warning);
                        }
                        listenOptions.UseHttps(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath.GetPath(configuration.BasePath), password);

                        X509Certificate2Collection collection = new X509Certificate2Collection();
                        collection.Import(configuration.WebAPIConfigurationVariables.WebServerSettings.TLSCertificatePFXFilePath.GetPath(configuration.BasePath), password, X509KeyStorageFlags.PersistKeySet);
                        List<X509Certificate2> certs = collection.ToList();
                        string dnsName = certs[0].GetNameInfo(X509NameType.DnsName, false);
                    }
                });
            });
            string appVersionString = "v" + configuration.WebAPIConfigurationConstants.AppVersion;

            builder.Services.AddControllers();
            if(HostAPIDocumentation(configuration.WebAPIConfigurationConstants.TargetEnvironmentType, configuration.WebAPIConfigurationVariables.WebServerSettings.HostAPISpecificationForInNonDevelopmentEnvironment, configuration.ExecutionMode))
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
            string url = $"{injectableSettings.Address}/{configuration.WebAPIConfigurationVariables.WebServerSettings.APIRoutePrefix}";
            return (app, url);
        }

        public class GetLoggerVisitor :IExecutionModeVisitor<IGeneralLogger>
        {
            private readonly GRYLogConfiguration _LogConfiguration;
            private readonly string _BaseFolder;
            public GetLoggerVisitor(GRYLogConfiguration logConfiguration, string baseFolder)
            {
                this._LogConfiguration = logConfiguration;
                this._BaseFolder = baseFolder;
            }
            public IGeneralLogger Handle(Analysis analysis)
            {
                return GeneralLogger.NoLog();// avoid creation of logging-entries when doing something like generate APISpecification-artifact by running "swagger tofile ..."
            }

            public IGeneralLogger Handle(RunProgram runProgram)
            {
                return GeneralLogger.CreateUsingGRYLog(this._LogConfiguration, this._BaseFolder);
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
            return GetDefaultLogConfiguration(AbstractFilePath.FromString(logFile), verbose, targetEnvironmentType);
        }
        public static GRYLogConfiguration GetDefaultLogConfiguration(AbstractFilePath logFile, bool verbose, GRYEnvironment targetEnvironmentType)
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