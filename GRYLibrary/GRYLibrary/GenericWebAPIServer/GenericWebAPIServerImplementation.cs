using GRYLibrary.Core.Log;
using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using GRYLibrary.Core.Miscellaneous;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    /// <remarks>
    /// When running and debugging CryptoCurrencyOnlineToolsNodeBitcoin locally the "Development"-configuration is supposed to be used.
    /// When running CryptoCurrencyOnlineToolsNodeBitcoin on a test-system in a docker-container the "QualityCheck"-configuration is supposed to be used.
    /// When running CryptoCurrencyOnlineToolsNodeBitcoin on a productive-system in a docker-container the "Productive"-configuration is supposed to be used.
    /// </remarks>
    public class GenericWebAPIServerImplementation<Startup, SettingsInterface, SettingsType>
        where Startup : AbstractStartup, new()
        where SettingsInterface : class, ISettingsInterface
        where SettingsType : SettingsInterface, new()
    {
        public IEnvironment Environment { get; private set; }
        public IConfiguration Configuration { get; private set; } = default;
        public SettingsInterface CurrentSettings { get; private set; } = default;
        public IAdministrationSettings AdministrationSettings { get; private set; } = default;
        public Action OnStart { get; set; } = () => { };
        public Action OnStop { get; set; } = () => { };
        public string ProgramName { get; set; } = default;
        public string ConfigurationFolder { get; set; } = default;
        public string DataFolder { get; set; } = default;
        public string LogFolder { get; set; } = default;
        public string AppSettingsFile { get; set; } = "AppSettings.json";
        public string AppSettingsSchemaFile { get; set; } = default;
        public GRYLog LogObject { get; private set; }
        public Version Version { get; set; }
        public string LogNamespaceForOverhead { get; set; } = "Server";
        public string LogNamespaceForLogMiddleware { get; set; } = "WebServerLog";
        public GenericWebAPIServerImplementation(string programName, Version version, IEnvironment environment)
        {
            this.ProgramName = programName;
            this.Version = version;
            this.Environment = environment;
        }
        public int Run()
        {
            try
            {
                Log(logObject => logObject.Log($"Started {ProgramName}", LogLevel.Debug), LogNamespaceForOverhead);

                string domainWithProtocol = $"https://{CurrentSettings.Domain}:{CurrentSettings.HTTPSPort}";

                WebHostBuilder webHostBuilder = new();
                webHostBuilder.UseEnvironment(Environment.GetType().Name);
                webHostBuilder.UseUrls(domainWithProtocol);
                webHostBuilder.UseKestrel(kestrelServerOptions =>
                {
                    kestrelServerOptions.ConfigureHttpsDefaults(httpsConnectionAdapterOptions2 =>
                    {
                        httpsConnectionAdapterOptions2.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                    });
                    var pfxFilePath = Utilities.NormalizePath(Path.Combine(ConfigurationFolder, CurrentSettings.CertificateFile));
                    var cvertificatePasswordFilePath = Utilities.NormalizePath(Path.Combine(ConfigurationFolder, CurrentSettings.CertificatePasswordFile));
                    X509Certificate2 certificate = new(pfxFilePath, File.ReadAllText(cvertificatePasswordFilePath, new UTF8Encoding(false)));
                    if (Environment is Productive && Utilities.IsSelfSIgned(certificate))
                    {
                        Log(logObject => logObject.LogWarning($"The used certificate '{CurrentSettings.CertificateFile}' is self-signed. Using self-signed certificates is not recommended in a productive environment."), LogNamespaceForOverhead);
                    }
                    kestrelServerOptions.AddServerHeader = false;
                    kestrelServerOptions.Limits.MaxRequestBodySize = CurrentSettings.MaxRequestBodySize;
                    kestrelServerOptions.Listen(System.Net.IPAddress.Loopback, CurrentSettings.HTTPSPort, listenOptions =>
                    {
                        listenOptions.UseHttps(certificate);
                    });
                });
                webHostBuilder.UseStartup<Startup>();

                IWebHost host = webHostBuilder.Build();
                OnStart();
                string apiExplorerAddress = $"{domainWithProtocol}/swagger/index.html";
                if (Environment is Development)
                {
                    Log(logObject => logObject.Log($"The API-explorer is available under the address '{apiExplorerAddress}'"), LogNamespaceForOverhead);
                }
                host.Run();

                OnStop();
                return 0;
            }
            catch (Exception exception)
            {
                Log(logObject => logObject.Log(exception, "Fatal error occurred"), LogNamespaceForOverhead);
                return 1;
            }
            finally
            {
                Log(logObject => logObject.Log($"Finished {ProgramName}", LogLevel.Debug), LogNamespaceForOverhead);
            }
        }

        private void ValidateAppSettings()
        {
            string appSettingsFile = Path.Combine(ConfigurationFolder, AppSettingsFile);
            if (File.Exists(appSettingsFile))
            {
                string appSettingsSchemaFile = Path.Combine(ConfigurationFolder, AppSettingsSchemaFile);
                if (string.IsNullOrWhiteSpace(appSettingsSchemaFile))
                {
                    Log(logObject => logObject.Log($"No json schema defined for app-settings-file.", LogLevel.Warning), LogNamespaceForOverhead);
                }
                else
                {
                    if (File.Exists(appSettingsSchemaFile))
                    {

                        Task<JsonSchema> schemaTask = JsonSchema.FromFileAsync(appSettingsSchemaFile);
                        schemaTask.Wait();
                        System.Collections.Generic.ICollection<NJsonSchema.Validation.ValidationError> errors = schemaTask.Result.Validate(File.ReadAllText(Path.Combine(ConfigurationFolder, appSettingsFile), new UTF8Encoding(false)));
                        if (errors.Count > 0)
                        {
                            Log(logObject => logObject.Log($"The appsettings are not matching the defined schema. The following errors occurred.", LogLevel.Error), LogNamespaceForOverhead);
                            foreach (NJsonSchema.Validation.ValidationError error in errors)
                            {
                                LogObject.Log($"Json validation error {Utilities.Format(error)}", LogLevel.Error);
                            }
                            throw new InvalidDataException($"The app-settings defined in '{appSettingsFile}' are not matching the schema defined in '{appSettingsSchemaFile}'.");
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException($"App-settings-schema-file '{appSettingsSchemaFile}' was not found.");
                    }
                }
            }
            else
            {
                throw new FileNotFoundException($"App-settings-file '{appSettingsFile}' was not found.");
            }
        }

        public void Initialize()
        {
            if (ConfigurationFolder != default)
            {
                Utilities.EnsureDirectoryExists(ConfigurationFolder);
            }
            if (DataFolder != default)
            {
                Utilities.EnsureDirectoryExists(DataFolder);
            }
            if (LogFolder != default)
            {
                Utilities.EnsureDirectoryExists(LogFolder);
            }
            LogObject = GRYLog.GetOrCreateAndGet($"{ConfigurationFolder}/Log.configuration", $"{LogFolder}/{ProgramName}_{DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss", CultureInfo.InvariantCulture)}.log");

            ValidateAppSettings();

            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder
                .SetBasePath(ConfigurationFolder)
                .AddJsonFile(Path.Combine(ConfigurationFolder, AppSettingsFile), optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();

            CurrentSettings = new SettingsType();
            Configuration.GetSection("Settings").Bind(CurrentSettings);
            this.AdministrationSettings = new AdministrationSettings(ProgramName, Version, Environment, ConfigurationFolder);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SettingsInterface>((_) => new SettingsType());
            services.AddSingleton<IAdministrationSettings>((_) => AdministrationSettings);
            services.AddControllers();
            //TODO: .AddAntiforgey()
            services.AddOpenApiDocument(); // add OpenAPI v3 document
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log(logObject => logObject.Log($"Current environment: {Environment.GetType().Name}", LogLevel.Information), LogNamespaceForOverhead);
            if (Environment is Development)
            {
                Utilities.AssertCondition(env.IsEnvironment(nameof(Development)));
            }

            if (Environment is QualityCheck)
            {
                Utilities.AssertCondition(env.IsEnvironment(nameof(QualityCheck)));
            }

            if (Environment is Productive)
            {
                Utilities.AssertCondition(env.IsEnvironment(nameof(Productive)));
                app.UseDDOSProtection();
                app.UseWebApplicationFirewall();
                app.UseObfuscation();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(); // serve OpenAPI/Swagger documents
                app.UseSwaggerUi3(); // serve Swagger UI
                app.UseReDoc(); // serve ReDoc UI
            }
            app.UseLog(log => Log(log, LogNamespaceForLogMiddleware));
            app.UseExceptionManager();
            if (Environment is Productive)
            {
                app.UseRequestCounter();
            }
            //app.UseHttpsRedirection();
            app.UseHsts();
            app.UseRouting();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }

        public string GetDefaultAppDataFolder(string applicationFolder, IEnvironment environment)
        {
            string prefix;
            if (environment is Development)
            {
                prefix = applicationFolder;
            }
            else
            {
                prefix = Path.DirectorySeparatorChar.ToString();
            }
            return prefix + "AppData";
        }
        public void Log(Action<GRYLog> log, string subNamespace)
        {
            lock (LogObject)
            {
                using (LogObject.UseSubNamespace(subNamespace))
                {
                    log(LogObject);
                }
            }
        }
    }
}
