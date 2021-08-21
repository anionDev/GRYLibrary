using GRYLibrary.Core.LogObject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer
{
    public class GenericWebAPIServerImplementation<Startup, SettingsInterface, SettingsType>
        where Startup : IStartup, new()
        where SettingsInterface : IWebserverSettingsInterface
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
            try
            {
                LogObject.Log($"Started {ProgramName}", LogLevel.Debug);

                ConfigurationBuilder builder = new();
                builder
                    .SetBasePath(ConfigurationFolder)
                    .AddJsonFile(AppSettingsFileName, optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();

                CurrentSettings = new SettingsType();
                Configuration.GetSection("Settings").Bind(CurrentSettings);

                WebHostBuilder hostBuilder = new();
                hostBuilder.UseKestrel(options =>
                    {
                        X509Certificate2 certificate = new(Path.Combine(Settings.Settings.GetConfigurationFolder(), CurrentSettings.CertificateFile), CurrentSettings.CertificatePassword);
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISettingsInterface>((_) => new SettingsType());
            services.AddSingleton<IAdministrationSettings>((_) => new AdministrationSettings(ProgramName, Version, Environment));
            services.AddControllers();
            //TODO: .AddAntiforgey()
            if (!(Environment is Productive))
            {
                services.AddOpenApiDocument((settings) =>
                {
                    settings.Title = ProgramName;
                    settings.Version = Version;
                });
            }

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
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            app.UseLog();
            app.UseExceptionManager();
            if (Environment is Productive)
            {
                app.UseRequestCounter();
            }
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
