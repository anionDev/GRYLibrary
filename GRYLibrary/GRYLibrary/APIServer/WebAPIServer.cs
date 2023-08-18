using GRYLibrary.Core.GeneralPurposeLogger;
using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Mid;
using GRYLibrary.Core.APIServer.Mid.Auth;
using GRYLibrary.Core.APIServer.Mid.Aut;
using GRYLibrary.Core.APIServer.Mid.Blacklist;
using GRYLibrary.Core.APIServer.Mid.Captcha;
using GRYLibrary.Core.APIServer.Mid.DDOS;
using GRYLibrary.Core.APIServer.Mid.Exception;
using GRYLibrary.Core.APIServer.Mid.Obfuscation;
using GRYLibrary.Core.APIServer.Mid.Counter;
using GRYLibrary.Core.APIServer.Mid.Logging;
using GRYLibrary.Core.APIServer.Mid.WAF;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.FilePath;
using GRYLibrary.Core.Miscellaneous.Migration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GRYLibrary.Core.Miscellaneous.ConsoleApplication;

namespace GRYLibrary.Core.APIServer
{
    /// <summary>
    /// Represents a webserver for a cloud-native HTTP-API-server
    /// </summary>
    public class WebAPIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
        where ApplicationSpecificConstants : new()
        where CommandlineParameterType : class, ICommandlineParameter
    {
        private APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> _APIServerInitializer;
        public static int WebAPIMain(CommandlineParameterType commandlineParameter, Action<ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>> apiServerInitializerConfigurator, GRYConsoleApplicationInitialInformation gryConsoleInitialInformation)
        {
            APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> apiServerInitializer = APIServerInitializer.Create<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>(gryConsoleInitialInformation);

            ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> configurationInformation = new ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
            {
                APIServerInitializer = apiServerInitializer,
                CommandlineParameter = commandlineParameter
            };

            apiServerInitializerConfigurator(configurationInformation);
            WebAPIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> server = new WebAPIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
            {
                _APIServerInitializer = apiServerInitializer
            };
            server._APIServerInitializer.ApplicationConstants.Initialize(server._APIServerInitializer.BaseFolder);
            return server.Run(configurationInformation);
        }


        public int Run(ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> configurationInformation)
        {
            IGeneralLogger consoleLogger = GeneralLogger.CreateUsingConsole();
            configurationInformation.Logger = consoleLogger;
            try
            {
                this.CreateRequiredFolder();
                this.RunMigrationIfRequired(configurationInformation.Logger, this._APIServerInitializer.BasicInformationFile);
                configurationInformation.Logger = this.GetApplicationLogger(configurationInformation.PersistedAPIServerConfiguration);
                configurationInformation.Logger.Log($"Start {this._APIServerInitializer.ApplicationConstants.ApplicationName}", LogLevel.Information);
                configurationInformation.Logger.Log($"Environment: {this._APIServerInitializer.ApplicationConstants.Environment}", LogLevel.Debug);
                configurationInformation.Logger.Log($"Executionmode: {this._APIServerInitializer.ApplicationConstants.ExecutionMode}", LogLevel.Debug);
                this.EnsureCertificateIsAvailableIfRequired(configurationInformation.PersistedAPIServerConfiguration, configurationInformation.Logger);
                WebApplication webApplication = this.CreateWebApplication(configurationInformation.PersistedAPIServerConfiguration, configurationInformation.Logger, configurationInformation.CommandlineParameter, configurationInformation);
                this._APIServerInitializer.PreRun();
                this.RunAPIServer(webApplication);
                this._APIServerInitializer.PostRun();
                return 0;
            }
            catch(Exception exception)
            {
                configurationInformation.Logger.LogException(exception, "Fatal error occurred.");
                return 1;
            }
        }

        private WebApplication CreateWebApplication(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration, IGeneralLogger logger,
            CommandlineParameterType commandlineParameter, ConfigurationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> configurationInformation)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = this._APIServerInitializer.ApplicationConstants.ApplicationName,
                EnvironmentName = this._APIServerInitializer.ApplicationConstants.Environment.GetType().Name
            });

            IMvcBuilder mvcBuilder = builder.Services.AddControllers();
            if(this._APIServerInitializer.ApplicationConstants.CommonRoutes is HostCommonRoutes)
            {
                mvcBuilder.AddApplicationPart(typeof(CommonRoutesController).Assembly);
            }

            builder.Services.AddSingleton((serviceProvider) => commandlineParameter);
            builder.Services.AddSingleton((serviceProvider) => logger);
            builder.Services.AddSingleton((serviceProvider) => persistedApplicationSpecificConfiguration);
            builder.Services.AddSingleton((serviceProvider) => this._APIServerInitializer.ApplicationConstants);
            builder.Services.AddSingleton<IApplicationConstants>((serviceProvider) => this._APIServerInitializer.ApplicationConstants);

            #region Load middlewares
            List<Type> middlewares = new List<Type>();
            List<Type> businessMiddleware = new List<Type>();

            #region General Threat-Protection
            if(this._APIServerInitializer.ApplicationConstants.Environment is not Development)
            {
                this.AddDefinedMiddleware((ISupportDDOSProtectionMiddleware c) => c.ConfigurationForDDOSProtection, this._APIServerInitializer.ApplicationConstants.DDOSProtectionMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportBlacklistMiddleware c) => c.ConfigurationForBlacklistMiddleware, this._APIServerInitializer.ApplicationConstants.BlackListMiddleware, persistedApplicationSpecificConfiguration, middlewares);
            }
            this.AddDefinedMiddleware((ISupportRequestLoggingMiddleware c) => c.ConfigurationForRequestLoggingMiddleware, this._APIServerInitializer.ApplicationConstants.RequestLoggingMiddleware, persistedApplicationSpecificConfiguration, middlewares);
            if(this._APIServerInitializer.ApplicationConstants.Environment is not Development)
            {
                this.AddDefinedMiddleware((ISupportWebApplicationFirewallMiddleware c) => c.ConfigurationForWebApplicationFirewall, this._APIServerInitializer.ApplicationConstants.WebApplicationFirewallMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportObfuscationMiddleware c) => c.ConfigurationForObfuscationMiddleware, this._APIServerInitializer.ApplicationConstants.ObfuscationMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportCaptchaMiddleware c) => c.ConfigurationForCaptchaMiddleware, this._APIServerInitializer.ApplicationConstants.CaptchaMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportExceptionManagerMiddleware c) => c.ConfigurationForExceptionManagerMiddleware, this._APIServerInitializer.ApplicationConstants.ExceptionManagerMiddleware, persistedApplicationSpecificConfiguration, middlewares);
            }
            #endregion

            #region Bussiness-implementation
            this.AddDefinedMiddleware((ISupportAuthenticationMiddleware c) => c.ConfigurationForAuthenticationMiddleware, this._APIServerInitializer.ApplicationConstants.AuthenticationMiddleware, persistedApplicationSpecificConfiguration, businessMiddleware);
            this.AddDefinedMiddleware((ISupportAuthorizationMiddleware c) => c.ConfigurationForAuthorizationMiddleware, this._APIServerInitializer.ApplicationConstants.AuthorizationMiddleware, persistedApplicationSpecificConfiguration, businessMiddleware);
            if(this._APIServerInitializer.ApplicationConstants.Environment is not Development)
            {
                this.AddDefinedMiddleware((ISupportRequestCounterMiddleware c) => c.ConfigurationForRequestCounterMiddleware, this._APIServerInitializer.ApplicationConstants.RequestCounterMiddleware, persistedApplicationSpecificConfiguration, businessMiddleware);
            }
            foreach(Type customMiddleware in this._APIServerInitializer.ApplicationConstants.CustomMiddlewares)
            {
                businessMiddleware.Add(customMiddleware);
            }
            #endregion
            #endregion

            configurationInformation.PersistedAPIServerConfiguration = persistedApplicationSpecificConfiguration;
            configurationInformation.ServiceCollection = builder.Services;

            this._APIServerInitializer.Configure(configurationInformation);
            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.AllowSynchronousIO = true;
                kestrelOptions.AddServerHeader = false;
                kestrelOptions.ListenAnyIP(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol.Port, listenOptions =>
                {
                    if(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS https)
                    {
                        string pfxFilePath = https.TLSCertificateInformation.CertificatePFXFile.GetPath(this._APIServerInitializer.ApplicationConstants.GetCertificateFolder());
                        string passwordFilePath = https.TLSCertificateInformation.CertificatePasswordFile.GetPath(this._APIServerInitializer.ApplicationConstants.GetCertificateFolder());
                        string password = File.ReadAllText(passwordFilePath, new UTF8Encoding(false));
                        X509Certificate2 certificate = certificate = new X509Certificate2(pfxFilePath, password);
                        if(this._APIServerInitializer.ApplicationConstants.Environment is Productive && Miscellaneous.Utilities.IsSelfSIgned(certificate))
                        {
                            logger.Log($"The used certificate '{pfxFilePath}' is self-signed. Using self-signed certificates is not recommended in a productive environment.", LogLevel.Warning);
                        }
                        listenOptions.UseHttps(certificate);
                        string dnsName = certificate.GetNameInfo(X509NameType.DnsName, false);
                        if(this._APIServerInitializer.ApplicationConstants.Environment is not Development && dnsName != persistedApplicationSpecificConfiguration.ServerConfiguration.Domain)
                        {
                            logger.Log($"The used certificate has the DNS-name '{dnsName}' which differs from the domain '{persistedApplicationSpecificConfiguration.ServerConfiguration.Domain}' which is set in the configuration.", LogLevel.Warning);
                        }
                    }
                });
            });

            string appVersionString = $"v{this._APIServerInitializer.ApplicationConstants.ApplicationVersion}";
            builder.Services.AddControllers(mvcOptions => mvcOptions.UseGeneralRoutePrefix(ServerConfiguration.APIRoutePrefix));
            builder.Services.AddControllers();
            bool hostAPIDocumentation = HostAPIDocumentation(this._APIServerInitializer.ApplicationConstants.Environment, persistedApplicationSpecificConfiguration.ServerConfiguration.HostAPISpecificationForInNonDevelopmentEnvironment, this._APIServerInitializer.ApplicationConstants.ExecutionMode);
            string apiUITitle = $"{this._APIServerInitializer.ApplicationConstants.ApplicationName} v{this._APIServerInitializer.ApplicationConstants.ApplicationVersion} API documentation";
            if(hostAPIDocumentation)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(swaggerOptions =>
                {
                    foreach(FilterDescriptor filter in this._APIServerInitializer.Filter)
                    {
                        swaggerOptions.OperationFilterDescriptors.Add(filter);
                    }
                    OpenApiInfo openAPIInfo = new OpenApiInfo
                    {
                        Version = appVersionString,
                        Title = apiUITitle,
                        Description = this._APIServerInitializer.ApplicationConstants.ApplicationDescription,
                    };
                    if(this._APIServerInitializer.ApplicationConstants.CommonRoutes is HostCommonRoutes)
                    {
                        openAPIInfo.TermsOfService = new Uri(persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + ServerConfiguration.TermsOfServiceURLSubPath);
                        openAPIInfo.Contact = new OpenApiContact
                        {
                            Name = "Contact",
                            Url = new Uri(persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + ServerConfiguration.ContactURLSubPath)
                        };
                        openAPIInfo.License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri(persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + ServerConfiguration.LicenseURLSubPath)
                        };
                    }
                    swaggerOptions.SwaggerDoc(ServerConfiguration.APISpecificationDocumentName, openAPIInfo);
                    string xmlFilename = $"{this._APIServerInitializer.ApplicationConstants.ApplicationName}.xml";
                    swaggerOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
            builder.Services.AddLogging(c => c.ClearProviders());
            WebApplication app = builder.Build();

            #region Add middlewares
            foreach(Type middleware in middlewares)
            {
                app.UseMiddleware(middleware);
            }
            if(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS)
            {
                app.UseHsts();
            }
            if(this._APIServerInitializer.ApplicationConstants.Environment is Development)
            {
                app.UseDeveloperExceptionPage();
            }
            foreach(Type middleware in businessMiddleware)
            {
                app.UseMiddleware(middleware);
            }
            #endregion

            #region API Documentation
            string apiLink = persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + ServerConfiguration.APIRoutePrefix;
            if(hostAPIDocumentation)
            {
                string openAPISpecificationRoute = $"/{ServerConfiguration.ResourcesSubPath}/{ServerConfiguration.APISpecificationDocumentName}";
                string apiDocumentationSubRoute = $"{ServerConfiguration.ResourcesSubPath}/{ServerConfiguration.APISpecificationDocumentName}";
                string entireAPIDocumentationRoute = $"{ServerConfiguration.APIRoutePrefix[1..]}/{apiDocumentationSubRoute}";

                app.UseSwagger(options => options.RouteTemplate = $"{entireAPIDocumentationRoute}/{{documentName}}/{this._APIServerInitializer.ApplicationConstants.ApplicationName}.api.json");
                app.UseSwaggerUI(options =>
                {
                    string appVersionString = $"v{this._APIServerInitializer.ApplicationConstants.ApplicationVersion}";
                    string ui = $"{ServerConfiguration.APISpecificationDocumentName}/{this._APIServerInitializer.ApplicationConstants.ApplicationName}.api.json";
                    options.SwaggerEndpoint(ui, this._APIServerInitializer.ApplicationConstants.ApplicationName + " " + appVersionString);
                    options.RoutePrefix = entireAPIDocumentationRoute;
                    options.DocumentTitle = apiUITitle;
                    apiLink = $"{apiLink}/{apiDocumentationSubRoute}/index.html";
                });
            }
            #endregion

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            logger.Log($"The API will now be available under the following URL:", LogLevel.Information);
            logger.Log(apiLink, LogLevel.Information);
            return app;
        }

        private void AddDefinedMiddleware<SupportDefinedMiddlewareType>(
            Func<SupportDefinedMiddlewareType, IMiddlewareConfiguration> getMiddlewareConfiguration,
            Type middlewareType,
            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration,
            List<Type> middlewares
        ) where SupportDefinedMiddlewareType : ISupportedMiddleware
        {
            if(persistedApplicationSpecificConfiguration.ApplicationSpecificConfiguration is SupportDefinedMiddlewareType supportDefinedMiddlewareType)
            {
                IMiddlewareConfiguration middlewareConfiguration = getMiddlewareConfiguration(supportDefinedMiddlewareType);
                if(middlewareConfiguration.Enabled)
                {
                    this._APIServerInitializer.Filter.UnionWith(middlewareConfiguration.GetFilter());
                    if(middlewareType == null)
                    {
                        throw new NullReferenceException($"No middleware-type given for {typeof(SupportDefinedMiddlewareType).FullName}.");
                    }
                    else
                    {
                        middlewares.Add(middlewareType);
                    }
                }
            }
        }

        #region Host API Documentation
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
                return true;// required for generation of OpenAPI-specification-json-file
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
        #endregion

        private void EnsureCertificateIsAvailableIfRequired(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration, IGeneralLogger logger)
        {
            string certFolder = this._APIServerInitializer.ApplicationConstants.GetCertificateFolder();
            if(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS https)
            {
                string pfxFile = https.TLSCertificateInformation.CertificatePFXFile.GetPath(certFolder);
                string passwordFile = https.TLSCertificateInformation.CertificatePasswordFile.GetPath(certFolder);
                if(!File.Exists(https.TLSCertificateInformation.CertificatePFXFile.GetPath(certFolder)))
                {
                    if(this._APIServerInitializer.ApplicationConstants.Environment is Productive)
                    {
                        logger.Log($"'{pfxFile}' does not exist. Fallback-certificate will be used instead. It is recommended to replace it by a valid certificate as soon as possible.", LogLevel.Warning);
                    }

                    if(https.TLSCertificateInformation.FallbackCertificatePasswordFileContentHex == null)
                    {
                        throw new ArgumentNullException($"{nameof(TLSCertificateInformation.FallbackCertificatePasswordFileContentHex)} is not allowed to be null if {nameof(HTTPS)} is used and no valid certificate is given.");
                    }
                    else
                    {
                        Miscellaneous.Utilities.EnsureFileExists(passwordFile, true);
                        File.WriteAllBytes(passwordFile, Miscellaneous.Utilities.HexStringToByteArray(https.TLSCertificateInformation.FallbackCertificatePasswordFileContentHex));
                    }

                    if(https.TLSCertificateInformation.FallbackCertificatePasswordFileContentHex == null)
                    {
                        throw new ArgumentNullException($"{nameof(TLSCertificateInformation.FallbackCertificatePFXFileContentHex)} is not allowed to be null if {nameof(HTTPS)} is used and no valid certificate is given.");
                    }
                    else
                    {
                        Miscellaneous.Utilities.EnsureFileExists(pfxFile, true);
                        File.WriteAllBytes(pfxFile, Miscellaneous.Utilities.HexStringToByteArray(https.TLSCertificateInformation.FallbackCertificatePFXFileContentHex));
                    }
                }
            }
        }

        private void RunMigrationIfRequired(IGeneralLogger logger, AbstractFilePath basicInformationFile)
        {
            GRYMigration.MigrateIfRequired(basicInformationFile, this._APIServerInitializer.ApplicationConstants.ApplicationName, this._APIServerInitializer.ApplicationConstants.ApplicationVersion, logger, this._APIServerInitializer.BaseFolder, this._APIServerInitializer.ApplicationConstants.Environment, this._APIServerInitializer.ApplicationConstants.ExecutionMode, new Dictionary<object, object>(), new HashSet<MigrationMetaInformation>());
        }

        private IGeneralLogger GetApplicationLogger(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration)
        {
            return this._APIServerInitializer.ApplicationConstants.ExecutionMode.Accept(new GetLoggerVisitor(persistedApplicationSpecificConfiguration.ApplicationLogConfiguration, this._APIServerInitializer.ApplicationConstants.GetLogFolder(), "Server"));
        }

        private void CreateRequiredFolder()
        {
            Miscellaneous.Utilities.EnsureDirectoryExists(this._APIServerInitializer.ApplicationConstants.GetConfigurationFolder());
            Miscellaneous.Utilities.EnsureDirectoryExists(this._APIServerInitializer.ApplicationConstants.GetLogFolder());
        }

        private void RunAPIServer(WebApplication server)
        {
            server.Run();
        }
    }
}
