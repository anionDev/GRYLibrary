using GRYLibrary.Core.LogObject;
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

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public class GenericWebAPIServerImplementation<Startup, SettingsInterface, SettingsType>
        where Startup : AbstractStartup, new()
        where SettingsInterface : class, ISettingsInterface
        where SettingsType : SettingsInterface, new()
    {
        public IEnvironment Environment { get; set; }
        public IConfiguration Configuration { get; private set; } = default;
        public SettingsInterface CurrentSettings { get; private set; } = default;
        public Action OnStart { get; set; } = () => { };
        public Action OnStop { get; set; } = () => { };
        public string ProgramName { get; set; } = default;
        public string ConfigurationFolder { get; set; } = default;
        public string DataFolder { get; set; } = default;
        public string LogFolder { get; set; } = default;
        public string AppSettingsFileName { get; set; } = "AppSettings.json";
        public GRYLog LogObject { get; private set; }
        public Version Version { get; set; }

        public GenericWebAPIServerImplementation(string programName, Version version)
        {
            this.ProgramName = programName;
            this.Version = version;
        }
        public int Run()
        {
            LogObject = GRYLog.GetOrCreateAndGet($"{ConfigurationFolder}/Log.configuration", $"{LogFolder}/{ProgramName}_{DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss", CultureInfo.InvariantCulture)}.log");
            try
            {
                LogObject.Log($"Started {ProgramName}", LogLevel.Debug);

                ConfigurationBuilder builder = new();
                builder
                    .SetBasePath(ConfigurationFolder)
                    .AddJsonFile(Path.Combine(ConfigurationFolder, AppSettingsFileName), optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();

                CurrentSettings = new SettingsType();
                Configuration.GetSection("Settings").Bind(CurrentSettings);

                WebHostBuilder hostBuilder = new();
                hostBuilder.UseKestrel(options =>
                    {
                        X509Certificate2 certificate = new(Path.Combine(ConfigurationFolder, CurrentSettings.CertificateFile), CurrentSettings.CertificatePassword);
                        if (Environment is Productive && Utilities.IsSelfSIgned(certificate))
                        {
                            LogObject.LogWarning($"The used certificate '{CurrentSettings.CertificateFile}' is self-signed. This is not recommended for a productive environment.");
                        }
                        options.AddServerHeader = false;
                        options.Limits.MaxRequestBodySize = CurrentSettings.MaxRequestBodySize;
                        options.Listen(System.Net.IPAddress.Loopback, CurrentSettings.HTTPSPort, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });
                    });
                hostBuilder.UseStartup<Startup>();

                IWebHost host = hostBuilder.Build();
                OnStart();
                string address = $"https://localhost:{CurrentSettings.HTTPSPort}/swagger/index.html";
                if (Environment is Development)
                {
                    LogObject.Log($"The API-explorer is available under the address '{address}'");
                
                }
                host.Run();

                OnStop();
                return 0;
            }
            catch (Exception exception)
            {
                LogObject.Log(exception, "Fatal error occurred");
                return 1;
            }
            finally
            {
                LogObject.Log($"Finished {ProgramName}", LogLevel.Debug);
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
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SettingsInterface>((_) => new SettingsType());
            services.AddSingleton<IAdministrationSettings>((_) => new AdministrationSettings(ProgramName, Version, Environment));
            services.AddControllers();
            //TODO: .AddAntiforgey()
            services.AddOpenApiDocument(); // add OpenAPI v3 document
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Environment is Development)
            {
                Utilities.AssertCondition(env.IsDevelopment());
                Utilities.AssertCondition(!env.IsProduction());
            }

            if (Environment is QualityCheck)
            {
                Utilities.AssertCondition(!env.IsDevelopment());
                Utilities.AssertCondition(!env.IsProduction());
            }

            if (Environment is Productive)
            {
                Utilities.AssertCondition(!env.IsDevelopment());
                Utilities.AssertCondition(env.IsProduction());
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
            app.UseLog();
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
    }
}
