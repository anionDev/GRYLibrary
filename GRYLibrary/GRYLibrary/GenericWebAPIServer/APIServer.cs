using GRYLibrary.Core.GeneralPurposeLogger;
using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.GenericWebAPIServer.Settings.CommonRoutes;
using GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration;
using GRYLibrary.Core.GenericWebAPIServer.Utilities;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.FilePath;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration;
using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class APIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>
        where PersistedApplicationSpecificConfiguration : new()
    {
        private readonly APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration> _APIServerInitializer;
        public APIServer(APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration> apiServerInitializer)
        {
            this._APIServerInitializer = apiServerInitializer;
            this._APIServerInitializer.ApplicationConstants.Initialize(this._APIServerInitializer.BaseFolder);
        }

        public static int WebAPIMain(CommandlineParameterType commandlineParameter, APIServerInitializer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration> initializer)
        {
            APIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType> server = new APIServer<ApplicationSpecificConstants, PersistedApplicationSpecificConfiguration, CommandlineParameterType>(initializer);
            return server.Run(commandlineParameter);
        }
        public int Run(CommandlineParameterType commandlineParameter)
        {
            IGeneralLogger logger = GeneralLogger.CreateUsingConsole();
            try
            {
                this.CreateRequiredFolder();
                this.RunMigrationIfRequired(logger, this._APIServerInitializer.BasicInformationFile);
                IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration = this.LoadConfiguration(this._APIServerInitializer.ApplicationConstants.KnownTypes);
                logger = this.GetApplicationLogger(persistedApplicationSpecificConfiguration);
                WebApplication server = this.Initialize(persistedApplicationSpecificConfiguration, logger, commandlineParameter);
                this._APIServerInitializer.PreRun(this._APIServerInitializer.ApplicationConstants, persistedApplicationSpecificConfiguration);
                this.RunAPIServer(server);
                this._APIServerInitializer.PostRun(this._APIServerInitializer.ApplicationConstants, persistedApplicationSpecificConfiguration);
                return 0;
            }
            catch(Exception exception)
            {
                logger.LogException(exception, "Fatal error occurred.");
                return 1;
            }
        }

        private WebApplication Initialize(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration, IGeneralLogger logger, CommandlineParameterType commandlineParameter)
        {
            logger.Log($"Start {this._APIServerInitializer.ApplicationConstants.ApplicationName}", LogLevel.Information);
            bool diagnosis = false;
            if(diagnosis)
            {
                logger.Log($"Environment: {this._APIServerInitializer.ApplicationConstants.Environment}", LogLevel.Debug);
                logger.Log($"Executionmode: {this._APIServerInitializer.ApplicationConstants.ExecutionMode}", LogLevel.Debug);
            }
            this.EnsureCertificateIsAvailableIfRequired(persistedApplicationSpecificConfiguration, logger);
            WebApplication webApplication = this.CreateAPIServer(persistedApplicationSpecificConfiguration, logger, commandlineParameter);
            return webApplication;
        }

        private WebApplication CreateAPIServer(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration, IGeneralLogger logger, CommandlineParameterType commandlineParameter)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = this._APIServerInitializer.ApplicationConstants.ApplicationName
            });

            IMvcBuilder mvcBuilder = builder.Services.AddControllers();
            if(this._APIServerInitializer.ApplicationConstants.CommonRoutes is HostCommonRoutes)
            {
                mvcBuilder.AddApplicationPart(typeof(OtherURLs).Assembly);
            }

            builder.Services.AddSingleton((serviceProvider) => logger);
            builder.Services.AddSingleton((serviceProvider) => persistedApplicationSpecificConfiguration);
            builder.Services.AddSingleton((serviceProvider) => this._APIServerInitializer.ApplicationConstants);
            builder.Services.AddSingleton<IApplicationConstants>((serviceProvider) => this._APIServerInitializer.ApplicationConstants);

            this._APIServerInitializer.ConfigureServices(builder.Services, this._APIServerInitializer.ApplicationConstants, persistedApplicationSpecificConfiguration);
            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.AddServerHeader = false;
                kestrelOptions.ListenAnyIP(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol.Port, listenOptions =>
                {
                    if(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS https)
                    {
                        string pfxFilePath = https.TLSCertificateInformation.CertificatePFXFile.GetPath(this._APIServerInitializer.ApplicationConstants.GetCertificateFolder());
                        string passwordFilePath = https.TLSCertificateInformation.CertificatePasswordFile.GetPath(this._APIServerInitializer.ApplicationConstants.GetCertificateFolder());
                        string password = File.ReadAllText(passwordFilePath, new UTF8Encoding(false));
                        X509Certificate2 certificate = new(pfxFilePath, password);
                        if(this._APIServerInitializer.ApplicationConstants.Environment is Productive && Miscellaneous.Utilities.IsSelfSIgned(certificate))
                        {
                            logger.Log($"The used certificate '{pfxFilePath}' is self-signed. Using self-signed certificates is not recommended in a productive environment.", LogLevel.Warning);
                        }
                        listenOptions.UseHttps(pfxFilePath, password);

                        X509Certificate2Collection collection = new X509Certificate2Collection();
                        collection.Import(pfxFilePath, password, X509KeyStorageFlags.PersistKeySet);
                        List<X509Certificate2> certs = collection.ToList();
                        string dnsName = certs[0].GetNameInfo(X509NameType.DnsName, false);
                        if(this._APIServerInitializer.ApplicationConstants.Environment is not Development && dnsName != persistedApplicationSpecificConfiguration.ServerConfiguration.Domain)
                        {
                            logger.Log($"The used certificate has the DNS-name '{dnsName}' which differs from the domain '{persistedApplicationSpecificConfiguration.ServerConfiguration.Domain}' which is set in the configuration.", LogLevel.Warning);
                        }
                    }
                });
            });
            string appVersionString = $"v{this._APIServerInitializer.ApplicationConstants.ApplicationVersion}";

            builder.Services.AddControllers();
            bool hostAPIDocumentation = HostAPIDocumentation(this._APIServerInitializer.ApplicationConstants.Environment, persistedApplicationSpecificConfiguration.ServerConfiguration.HostAPISpecificationForInNonDevelopmentEnvironment, this._APIServerInitializer.ApplicationConstants.ExecutionMode);
            if(hostAPIDocumentation)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(swaggerOptions =>
                {
                    foreach(FilterDescriptor filter in this._APIServerInitializer.Filter)
                    {
                        swaggerOptions.OperationFilterDescriptors.Add(filter);
                    }
                    swaggerOptions.SwaggerDoc(persistedApplicationSpecificConfiguration.ServerConfiguration.APIDocumentationDocumentName, new OpenApiInfo
                    {
                        Version = appVersionString,
                        Title = $"{this._APIServerInitializer.ApplicationConstants.ApplicationName} API",
                        Description = this._APIServerInitializer.ApplicationConstants.ApplicationDescription,
                        TermsOfService = new Uri(persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + persistedApplicationSpecificConfiguration.ServerConfiguration.TermsOfServiceURLSubPath),
                        Contact = new OpenApiContact
                        {
                            Name = "Contact",
                            Url = new Uri(persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + persistedApplicationSpecificConfiguration.ServerConfiguration.ContactURLSubPath)
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri(persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress() + persistedApplicationSpecificConfiguration.ServerConfiguration.LicenseURLSubPath)
                        }
                    });
                    string xmlFilename = $"{this._APIServerInitializer.ApplicationConstants.ApplicationName}.xml";
                    swaggerOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
            builder.Services.AddLogging(c => c.ClearProviders());
            builder.Services.AddControllers(mvcOptions => mvcOptions.UseGeneralRoutePrefix(ServerConfiguration.GetAPIDocumentationRoutePrefix()));
            WebApplication app = builder.Build();

            #region General Threat-Protection
            if(this._APIServerInitializer.ApplicationConstants.Environment is not Development)
            {
                if(this._APIServerInitializer.ApplicationConstants.DDOSProtectionMiddleware != null)
                {
                    app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.DDOSProtectionMiddleware);
                }
                if(this._APIServerInitializer.ApplicationConstants.BlackListMiddleware != null)
                {
                    app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.BlackListMiddleware);
                }
                if(this._APIServerInitializer.ApplicationConstants.ObfuscationMiddleware != null)
                {
                    app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.ObfuscationMiddleware);
                }
                if(this._APIServerInitializer.ApplicationConstants.ExceptionManagerMiddleware != null)
                {
                    app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.ExceptionManagerMiddleware);
                }
                if(this._APIServerInitializer.ApplicationConstants.CaptchaMiddleware != null)
                {
                    app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.CaptchaMiddleware);
                }
            }
            app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.CaptchaMiddleware);
            if(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS)
            {
                app.UseHsts();
            }
            #endregion

            #region Diagnosis
            if(this._APIServerInitializer.ApplicationConstants.RequestLoggingMiddleware != null)
            {
                app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.RequestLoggingMiddleware);
            }
            if(this._APIServerInitializer.ApplicationConstants.Environment is Development)
            {
                app.UseDeveloperExceptionPage();
            }
            #endregion

            #region Bussiness-implementation of access-restriction
            if(this._APIServerInitializer.ApplicationConstants.WebApplicationFirewallMiddleware != null)
            {
                app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.WebApplicationFirewallMiddleware);
            }
            if(this._APIServerInitializer.ApplicationConstants.ApiKeyValidatorMiddleware != null)
            {
                app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.ApiKeyValidatorMiddleware);
            }
            if(this._APIServerInitializer.ApplicationConstants.AuthenticationMiddleware != null)
            {
                app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.AuthenticationMiddleware);
            }
            if(this._APIServerInitializer.ApplicationConstants.AuthorizationMiddleware != null)
            {
                app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.AuthorizationMiddleware);
            }
            if(this._APIServerInitializer.ApplicationConstants.Environment is not Development)
            {
                if(this._APIServerInitializer.ApplicationConstants.RequestCounterMiddleware != null)
                {
                    app.UseMiddleware(this._APIServerInitializer.ApplicationConstants.RequestCounterMiddleware);
                }
            }
            #endregion

            #region API Documentation

            if(hostAPIDocumentation)
            {
                app.UseSwagger(options => options.RouteTemplate = $"{ServerConfiguration.GetAPIDocumentationRoutePrefix()}/Other/Resources/{{documentName}}/{this._APIServerInitializer.ApplicationConstants.ApplicationName}.api.json");
                app.UseSwaggerUI(options =>
                {
                    string appVersionString = $"v{this._APIServerInitializer.ApplicationConstants.ApplicationVersion}";
                    options.SwaggerEndpoint($"Other/Resources/{persistedApplicationSpecificConfiguration.ServerConfiguration.APIDocumentationDocumentName}/{this._APIServerInitializer.ApplicationConstants.ApplicationName}.api.json", this._APIServerInitializer.ApplicationConstants.ApplicationName + " " + appVersionString);
                    options.RoutePrefix = ServerConfiguration.GetAPIDocumentationRoutePrefix();
                });
            }

            string url = $"{persistedApplicationSpecificConfiguration.ServerConfiguration.GetServerAddress()}/{ServerConfiguration.GetAPIDocumentationRoutePrefix()}";

            string urlSuffix;
            if(HostAPIDocumentation(this._APIServerInitializer.ApplicationConstants.Environment, hostAPIDocumentation, this._APIServerInitializer.ApplicationConstants.ExecutionMode))
            {
                urlSuffix = "/index.html";
            }
            else
            {
                urlSuffix = string.Empty;
            }
            logger.Log($"The API is available under the following URL:", LogLevel.Debug);
            logger.Log(url + urlSuffix, LogLevel.Debug);

            #endregion

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            return app;
        }

        private void EnsureCertificateIsAvailableIfRequired(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration, IGeneralLogger logger)
        {
            string certFolder = this._APIServerInitializer.ApplicationConstants.GetCertificateFolder();
            if(persistedApplicationSpecificConfiguration.ServerConfiguration.Protocol is HTTPS https)
            {
                string pfxFile = https.TLSCertificateInformation.CertificatePFXFile.GetPath(certFolder);
                string passwordFile = https.TLSCertificateInformation.CertificatePasswordFile.GetPath(certFolder);
                if(!File.Exists(https.TLSCertificateInformation.CertificatePFXFile.GetPath(certFolder)))
                {
                    Miscellaneous.Utilities.EnsureFileExists(passwordFile, true);
                    File.WriteAllBytes(passwordFile, Miscellaneous.Utilities.HexStringToByteArray(https.TLSCertificateInformation.FallbackCertificatePasswordFileContentHex));

                    Miscellaneous.Utilities.EnsureFileExists(pfxFile, true);
                    File.WriteAllBytes(pfxFile, Miscellaneous.Utilities.HexStringToByteArray(https.TLSCertificateInformation.FallbackCertificatePFXFileContentHex));

                    if(this._APIServerInitializer.ApplicationConstants.Environment is Productive)
                    {
                        logger.Log($"'{pfxFile}' does not exist. Nonproductive-certificate will be used instead. It is recommended to replace it by a productive-certificate as soon as possible.", LogLevel.Warning);
                    }
                }
            }
        }

        #region Get logger
        private IGeneralLogger GetApplicationLogger(IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> persistedApplicationSpecificConfiguration)
        {
            return this._APIServerInitializer.ApplicationConstants.ExecutionMode.Accept(new GetLoggerVisitor(persistedApplicationSpecificConfiguration.ApplicationLogConfiguration, this._APIServerInitializer.ApplicationConstants.GetLogFolder(), "Server"));
        }

        #endregion

        private void CreateRequiredFolder()
        {
            Miscellaneous.Utilities.EnsureDirectoryExists(this._APIServerInitializer.ApplicationConstants.GetConfigurationFolder());
            Miscellaneous.Utilities.EnsureDirectoryExists(this._APIServerInitializer.ApplicationConstants.GetLogFolder());
        }

        private void RunAPIServer(WebApplication server)
        {
            server.Run();
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

        #endregion

        #region Create or load config-file

        private IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> LoadConfiguration(ISet<Type> knownTypes)
        {
            IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> result = this._APIServerInitializer.ApplicationConstants.ExecutionMode.Accept(new GetPersistedAPIServerConfigurationVisitor(this._APIServerInitializer.ApplicationConstants.GetConfigurationFile(), this._APIServerInitializer.InitialApplicationConfiguration, knownTypes));
            return result;
        }
        private class GetPersistedAPIServerConfigurationVisitor :IExecutionModeVisitor<IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>>
        {
            private readonly MetaConfigurationSettings<PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>> _MetaConfiguration;
            private readonly ISet<Type> _KnownTypes;

            public GetPersistedAPIServerConfigurationVisitor(string file, PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> initialValue, ISet<Type> knownTypes)
            {
                this._MetaConfiguration = new MetaConfigurationSettings<PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>, IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>>()
                {
                    ConfigurationFormat = XML.Instance,
                    File = file,
                    InitialValue = initialValue
                };
                this._KnownTypes = knownTypes;
            }

            public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> Handle(Analysis analysis)
            {
                return this._MetaConfiguration.InitialValue;
            }

            public IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> Handle(RunProgram runProgram)
            {
                return MetaConfigurationManager.GetConfiguration(this._MetaConfiguration, this._KnownTypes);
            }
        }
        #endregion
        private void RunMigrationIfRequired(IGeneralLogger logger, AbstractFilePath basicInformationFile)
        {
            GRYMigration.MigrateIfRequired(basicInformationFile, this._APIServerInitializer.ApplicationConstants.ApplicationName, this._APIServerInitializer.ApplicationConstants.ApplicationVersion, logger, this._APIServerInitializer.BaseFolder, this._APIServerInitializer.ApplicationConstants.Environment, this._APIServerInitializer.ApplicationConstants.ExecutionMode, new Dictionary<object, object>(), new HashSet<MigrationMetaInformation>());
        }
    }
}
