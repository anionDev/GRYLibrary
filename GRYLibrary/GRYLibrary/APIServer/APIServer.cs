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
using GRYLibrary.Core.APIServer.ExecutionModes.Visitors;
using System.Reflection;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration;

namespace GRYLibrary.Core.APIServer
{
    /// <summary>
    /// Represents a webserver for a cloud-native HTTP-API-server
    /// </summary>
    public class APIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
        where ApplicationSpecificConstants : new()
        where CommandlineParameterType : class, ICommandlineParameter
    {
        private APIServerConfiguration<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> _Configuration;
        public static int APIMain(CommandlineParameterType commandlineParameter, Action<APIServerConfiguration<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>> configurationInitializer, GRYConsoleApplicationInitialInformation gryConsoleApplicationInitialInformation)
        {
            #region Obsolete
            string domain = default;//TODO set value (this value can probably be derived by the environment-variable and a development-certificate-name)
            string fallbackCertificatePasswordFileContentHex = default;//TODO set value and make sure it is not a fallback anymore
            string fallbackCertificatePFXFileContentHex = default;//TODO set value and make sure it is not a fallback anymore
            #endregion

            #region Initialize default configuration-values
            APIServerConfiguration<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> apiServerConfiguration = new APIServerConfiguration<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>();
            configurationInitializer(apiServerConfiguration);
            apiServerConfiguration.InitializationInformation = new InitializationInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
            {
                CommandlineParameter = commandlineParameter,
                ApplicationConstants = new ApplicationConstants<ApplicationSpecificConstants>(gryConsoleApplicationInitialInformation.ProgramName, gryConsoleApplicationInitialInformation.ProgramDescription, Version3.Parse(gryConsoleApplicationInitialInformation.ProgramVersion), gryConsoleApplicationInitialInformation.ExecutionMode, gryConsoleApplicationInitialInformation.Environment, new ApplicationSpecificConstants())
            };
            apiServerConfiguration.InitializationInformation.BaseFolder = GetDefaultBaseFolder(apiServerConfiguration.InitializationInformation.ApplicationConstants);
            apiServerConfiguration.InitializationInformation.ApplicationConstants.Initialize(apiServerConfiguration.InitializationInformation.BaseFolder);
            apiServerConfiguration.InitializationInformation.ApplicationConstants.KnownTypes.Add(typeof(PersistedApplicationSpecificConfiguration));
            apiServerConfiguration.InitializationInformation.InitialApplicationConfiguration = PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>.Create(domain, new PersistedApplicationSpecificConfiguration(), gryConsoleApplicationInitialInformation.Environment, fallbackCertificatePasswordFileContentHex, fallbackCertificatePFXFileContentHex, apiServerConfiguration.InitializationInformation.ApplicationConstants.ApplicationName);
            apiServerConfiguration.InitializationInformation.BasicInformationFile = AbstractFilePath.FromString("./BasicApplicationInformation.xml");
            #endregion

            #region Run initialization-function
            apiServerConfiguration.SetInitialzationInformationAction(apiServerConfiguration.InitializationInformation);
            #endregion

            #region Load configuration
            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedAPIServerConfiguration = LoadConfiguration(apiServerConfiguration.InitializationInformation.ApplicationConstants.KnownTypes, apiServerConfiguration.InitializationInformation.ApplicationConstants.Environment, apiServerConfiguration.InitializationInformation.ApplicationConstants.ExecutionMode, apiServerConfiguration.InitializationInformation.ApplicationConstants.GetConfigurationFile(), apiServerConfiguration.InitializationInformation.ApplicationConstants.ThrowErrorIfConfigurationDoesNotExistInProduction, apiServerConfiguration.InitializationInformation.InitialApplicationConfiguration);
            #endregion

            #region Run APIServer
            APIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> server = new APIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
            {
                _Configuration = apiServerConfiguration
            };
            return server.Run(apiServerConfiguration, persistedAPIServerConfiguration);
            #endregion
        }
        private static string GetDefaultBaseFolder<AppConstantsType>(IApplicationConstants<AppConstantsType> applicationConstants)
        {
            string programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return applicationConstants.ExecutionMode.Accept(new GetBaseFolder(applicationConstants.Environment, programFolder));
        }

        #region Create or load config-file

        internal static IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> LoadConfiguration<PersistedAppSpecificConfiguration>(
            ISet<Type> knownTypes, GRYEnvironment evironment, ExecutionMode executionMode, string configurationFile, bool throwErrorIfConfigurationDoesNotExistInProduction,
            PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> initialConfiguration)
                where PersistedAppSpecificConfiguration : new()
        {
            if (throwErrorIfConfigurationDoesNotExistInProduction && evironment is Productive && !File.Exists(configurationFile))
            {
                throw new FileNotFoundException($"Configurationfile \"{configurationFile}\" does not exist.");
            }
            else
            {
                IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> result = executionMode.Accept(new GetPersistedAPIServerConfigurationVisitor<PersistedAppSpecificConfiguration>(configurationFile, initialConfiguration, knownTypes));
                return result;
            }
        }
        private class GetPersistedAPIServerConfigurationVisitor<PersistedAppSpecificConfiguration> : IExecutionModeVisitor<IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>>
                where PersistedAppSpecificConfiguration : new()
        {
            private readonly MetaConfigurationSettings<PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>, IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>> _MetaConfiguration;
            private readonly ISet<Type> _KnownTypes;

            public GetPersistedAPIServerConfigurationVisitor(string file, PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> initialValue, ISet<Type> knownTypes)
            {
                this._MetaConfiguration = new MetaConfigurationSettings<PersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>, IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration>>()
                {
                    ConfigurationFormat = XML.Instance,
                    File = file,
                    InitialValue = initialValue
                };
                this._KnownTypes = knownTypes;
            }

            public IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> Handle(Analysis analysis)
            {
                return this._MetaConfiguration.InitialValue;
            }

            public IPersistedAPIServerConfiguration<PersistedAppSpecificConfiguration> Handle(RunProgram runProgram)
            {
                return MetaConfigurationManager.GetConfiguration(this._MetaConfiguration, this._KnownTypes);
            }
        }
        #endregion

        public int Run(APIServerConfiguration<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> config, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedAPIServerConfiguration)
        {
            IGeneralLogger logger = GeneralLogger.CreateUsingConsole();
            try
            {
                this.CreateRequiredFolder();
                this.RunMigrationIfRequired(logger, this._Configuration.InitializationInformation.BasicInformationFile);
                logger = this.GetApplicationLogger(persistedAPIServerConfiguration);
                logger.Log($"Start {this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName}", LogLevel.Information);
                logger.Log($"Environment: {this._Configuration.InitializationInformation.ApplicationConstants.Environment}", LogLevel.Debug);
                logger.Log($"Executionmode: {this._Configuration.InitializationInformation.ApplicationConstants.ExecutionMode}", LogLevel.Debug);
                this.EnsureCertificateIsAvailableIfRequired(persistedAPIServerConfiguration, logger);
                WebApplication webApplication = this.CreateWebApplication(config, logger, persistedAPIServerConfiguration);
                this._Configuration.FunctionalInformation.PreRun();
                this.RunAPIServer(webApplication);
                this._Configuration.FunctionalInformation.PostRun();
                return 0;
            }
            catch (Exception exception)
            {
                logger.LogException(exception, "Fatal error occurred.");
                return 1;
            }
        }

        private WebApplication CreateWebApplication(
            APIServerConfiguration<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> apiServerConfiguration,
            IGeneralLogger logger,
            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedAPIServerConfiguration)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName,
                EnvironmentName = this._Configuration.InitializationInformation.ApplicationConstants.Environment.GetType().Name
            });
            IMvcBuilder mvcBuilder = builder.Services.AddControllers();
            if (this._Configuration.InitializationInformation.ApplicationConstants.CommonRoutes is HostCommonRoutes)
            {
                mvcBuilder.AddApplicationPart(typeof(CommonRoutesController).Assembly);
            }

            builder.Services.AddSingleton((serviceProvider) => apiServerConfiguration.InitializationInformation.CommandlineParameter);
            builder.Services.AddSingleton((serviceProvider) => logger);
            builder.Services.AddSingleton((serviceProvider) => persistedAPIServerConfiguration);
            builder.Services.AddSingleton((serviceProvider) => this._Configuration.InitializationInformation.ApplicationConstants);
            builder.Services.AddSingleton<IApplicationConstants>((serviceProvider) => this._Configuration.InitializationInformation.ApplicationConstants);

            apiServerConfiguration.FunctionalInformation = new FunctionalInformation<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>(
                apiServerConfiguration.InitializationInformation,
                builder.Services,
                persistedAPIServerConfiguration
            );
            apiServerConfiguration.SetFunctionalInformationAction(apiServerConfiguration.FunctionalInformation);

            #region Load middlewares
            List<Type> middlewares = new List<Type>();
            List<Type> businessMiddleware = new List<Type>();

            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration = apiServerConfiguration.FunctionalInformation.PersistedAPIServerConfiguration;
            #region General Threat-Protection
            if (this._Configuration.InitializationInformation.ApplicationConstants.Environment is not Development)
            {
                this.AddDefinedMiddleware((ISupportDDOSProtectionMiddleware c) => c.ConfigurationForDDOSProtection, this._Configuration.InitializationInformation.ApplicationConstants.DDOSProtectionMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportBlacklistMiddleware c) => c.ConfigurationForBlacklistMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.BlackListMiddleware, persistedApplicationSpecificConfiguration, middlewares);
            }
            this.AddDefinedMiddleware((ISupportRequestLoggingMiddleware c) => c.ConfigurationForRequestLoggingMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.RequestLoggingMiddleware, persistedApplicationSpecificConfiguration, middlewares);
            if (this._Configuration.InitializationInformation.ApplicationConstants.Environment is not Development)
            {
                this.AddDefinedMiddleware((ISupportWebApplicationFirewallMiddleware c) => c.ConfigurationForWebApplicationFirewall, this._Configuration.InitializationInformation.ApplicationConstants.WebApplicationFirewallMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportObfuscationMiddleware c) => c.ConfigurationForObfuscationMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.ObfuscationMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportCaptchaMiddleware c) => c.ConfigurationForCaptchaMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.CaptchaMiddleware, persistedApplicationSpecificConfiguration, middlewares);
                this.AddDefinedMiddleware((ISupportExceptionManagerMiddleware c) => c.ConfigurationForExceptionManagerMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.ExceptionManagerMiddleware, persistedApplicationSpecificConfiguration, middlewares);
            }
            #endregion

            #region Bussiness-implementation
            this.AddDefinedMiddleware((ISupportAuthenticationMiddleware c) => c.ConfigurationForAuthenticationMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.AuthenticationMiddleware, persistedApplicationSpecificConfiguration, businessMiddleware);
            this.AddDefinedMiddleware((ISupportAuthorizationMiddleware c) => c.ConfigurationForAuthorizationMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.AuthorizationMiddleware, persistedApplicationSpecificConfiguration, businessMiddleware);
            if (this._Configuration.InitializationInformation.ApplicationConstants.Environment is not Development)
            {
                this.AddDefinedMiddleware((ISupportRequestCounterMiddleware c) => c.ConfigurationForRequestCounterMiddleware, this._Configuration.InitializationInformation.ApplicationConstants.RequestCounterMiddleware, persistedApplicationSpecificConfiguration, businessMiddleware);
            }
            foreach (Type customMiddleware in this._Configuration.InitializationInformation.ApplicationConstants.CustomMiddlewares)
            {
                businessMiddleware.Add(customMiddleware);
            }
            #endregion
            #endregion

            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.AllowSynchronousIO = true;
                kestrelOptions.AddServerHeader = false;
                kestrelOptions.ListenAnyIP(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol.Port, listenOptions =>
                {
                    if (persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS https)
                    {
                        string pfxFilePath = https.TLSCertificateInformation.CertificatePFXFile.GetPath(this._Configuration.InitializationInformation.ApplicationConstants.GetCertificateFolder());
                        string passwordFilePath = https.TLSCertificateInformation.CertificatePasswordFile.GetPath(this._Configuration.InitializationInformation.ApplicationConstants.GetCertificateFolder());
                        string password = File.ReadAllText(passwordFilePath, new UTF8Encoding(false));
                        X509Certificate2 certificate = certificate = new X509Certificate2(pfxFilePath, password);
                        if (this._Configuration.InitializationInformation.ApplicationConstants.Environment is Productive && Miscellaneous.Utilities.IsSelfSIgned(certificate))
                        {
                            logger.Log($"The used certificate '{pfxFilePath}' is self-signed. Using self-signed certificates is not recommended in a productive environment.", LogLevel.Warning);
                        }
                        listenOptions.UseHttps(certificate);
                        string dnsName = certificate.GetNameInfo(X509NameType.DnsName, false);
                        if (this._Configuration.InitializationInformation.ApplicationConstants.Environment is not Development && dnsName != persistedApplicationSpecificConfiguration.ServerConfiguration.Domain)
                        {
                            logger.Log($"The used certificate has the DNS-name '{dnsName}' which differs from the domain '{persistedApplicationSpecificConfiguration.ServerConfiguration.Domain}' which is set in the configuration.", LogLevel.Warning);
                        }
                    }
                });
            });

            string appVersionString = $"v{this._Configuration.InitializationInformation.ApplicationConstants.ApplicationVersion}";
            builder.Services.AddControllers(mvcOptions => mvcOptions.UseGeneralRoutePrefix(ServerConfiguration.APIRoutePrefix));
            builder.Services.AddControllers();
            bool hostAPIDocumentation = HostAPIDocumentation(this._Configuration.InitializationInformation.ApplicationConstants.Environment, persistedApplicationSpecificConfiguration.ServerConfiguration.HostAPISpecificationForInNonDevelopmentEnvironment, this._Configuration.InitializationInformation.ApplicationConstants.ExecutionMode);
            string apiUITitle = $"{this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName} v{this._Configuration.InitializationInformation.ApplicationConstants.ApplicationVersion} API documentation";
            if (hostAPIDocumentation)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(swaggerOptions =>
                {
                    foreach (FilterDescriptor filter in this._Configuration.FunctionalInformation.Filter)
                    {
                        swaggerOptions.OperationFilterDescriptors.Add(filter);
                    }
                    OpenApiInfo openAPIInfo = new OpenApiInfo
                    {
                        Version = appVersionString,
                        Title = apiUITitle,
                        Description = this._Configuration.InitializationInformation.ApplicationConstants.ApplicationDescription,
                    };
                    if (this._Configuration.InitializationInformation.ApplicationConstants.CommonRoutes is HostCommonRoutes)
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
                    string xmlFilename = $"{this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName}.xml";
                    swaggerOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
            builder.Services.AddLogging(c => c.ClearProviders());
            WebApplication app = builder.Build();

            #region Add middlewares
            foreach (Type middleware in middlewares)
            {
                app.UseMiddleware(middleware);
            }
            if (persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS)
            {
                app.UseHsts();
            }
            if (this._Configuration.InitializationInformation.ApplicationConstants.Environment is Development)
            {
                app.UseDeveloperExceptionPage();
            }
            foreach (Type middleware in businessMiddleware)
            {
                app.UseMiddleware(middleware);
            }
            #endregion

            #region API Documentation
            string apiLink = persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + ServerConfiguration.APIRoutePrefix;
            if (hostAPIDocumentation)
            {
                string openAPISpecificationRoute = $"/{ServerConfiguration.ResourcesSubPath}/{ServerConfiguration.APISpecificationDocumentName}";
                string apiDocumentationSubRoute = $"{ServerConfiguration.ResourcesSubPath}/{ServerConfiguration.APISpecificationDocumentName}";
                string entireAPIDocumentationRoute = $"{ServerConfiguration.APIRoutePrefix[1..]}/{apiDocumentationSubRoute}";

                app.UseSwagger(options => options.RouteTemplate = $"{entireAPIDocumentationRoute}/{{documentName}}/{this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName}.api.json");
                app.UseSwaggerUI(options =>
                {
                    string appVersionString = $"v{this._Configuration.InitializationInformation.ApplicationConstants.ApplicationVersion}";
                    string ui = $"{ServerConfiguration.APISpecificationDocumentName}/{this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName}.api.json";
                    options.SwaggerEndpoint(ui, this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName + " " + appVersionString);
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
            if (persistedApplicationSpecificConfiguration.ApplicationSpecificConfiguration is SupportDefinedMiddlewareType supportDefinedMiddlewareType)
            {
                IMiddlewareConfiguration middlewareConfiguration = getMiddlewareConfiguration(supportDefinedMiddlewareType);
                if (middlewareConfiguration.Enabled)
                {
                    this._Configuration.FunctionalInformation.Filter.UnionWith(middlewareConfiguration.GetFilter());
                    if (middlewareType == null)
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
        private class GetHostAPIDocumentationVisitor : IExecutionModeVisitor<bool>
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
                if (this._Environment is Development)
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
            string certFolder = this._Configuration.InitializationInformation.ApplicationConstants.GetCertificateFolder();
            if (persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS https)
            {
                string pfxFile = https.TLSCertificateInformation.CertificatePFXFile.GetPath(certFolder);
                string passwordFile = https.TLSCertificateInformation.CertificatePasswordFile.GetPath(certFolder);
                if (!File.Exists(https.TLSCertificateInformation.CertificatePFXFile.GetPath(certFolder)))
                {
                    if (this._Configuration.InitializationInformation.ApplicationConstants.Environment is Productive)
                    {
                        logger.Log($"'{pfxFile}' does not exist. Fallback-certificate will be used instead. It is recommended to replace it by a valid certificate as soon as possible.", LogLevel.Warning);
                    }

                    if (https.TLSCertificateInformation.FallbackCertificatePasswordFileContentHex == null)
                    {
                        throw new ArgumentNullException($"{nameof(TLSCertificateInformation.FallbackCertificatePasswordFileContentHex)} is not allowed to be null if {nameof(HTTPS)} is used and no valid certificate is given.");
                    }
                    else
                    {
                        Miscellaneous.Utilities.EnsureFileExists(passwordFile, true);
                        File.WriteAllBytes(passwordFile, Miscellaneous.Utilities.HexStringToByteArray(https.TLSCertificateInformation.FallbackCertificatePasswordFileContentHex));
                    }

                    if (https.TLSCertificateInformation.FallbackCertificatePasswordFileContentHex == null)
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
            GRYMigration.MigrateIfRequired(basicInformationFile, this._Configuration.InitializationInformation.ApplicationConstants.ApplicationName, this._Configuration.InitializationInformation.ApplicationConstants.ApplicationVersion, logger, this._Configuration.InitializationInformation.BaseFolder, this._Configuration.InitializationInformation.ApplicationConstants.Environment, this._Configuration.InitializationInformation.ApplicationConstants.ExecutionMode, new Dictionary<object, object>(), new HashSet<MigrationMetaInformation>());
        }

        private IGeneralLogger GetApplicationLogger(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration)
        {
            return this._Configuration.InitializationInformation.ApplicationConstants.ExecutionMode.Accept(new GetLoggerVisitor(persistedApplicationSpecificConfiguration.ApplicationLogConfiguration, this._Configuration.InitializationInformation.ApplicationConstants.GetLogFolder(), "Server"));
        }

        private void CreateRequiredFolder()
        {
            Miscellaneous.Utilities.EnsureDirectoryExists(this._Configuration.InitializationInformation.ApplicationConstants.GetConfigurationFolder());
            Miscellaneous.Utilities.EnsureDirectoryExists(this._Configuration.InitializationInformation.ApplicationConstants.GetLogFolder());
        }

        private void RunAPIServer(WebApplication server)
        {
            server.Run();
        }
    }
}
