﻿using GRYLibrary.Core.Log;
using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
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
using System.Collections.Generic;
using GRYLibrary.Core.GenericWebAPIServer.Settings;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    /// <remarks>
    /// When running and debugging locally the <see cref="Development"/>-configuration is supposed to be used.
    /// When running on a test-system the <see cref="QualityCheck"/>-configuration is supposed to be used.
    /// When running on a productive-system the <see cref="Productive"/>-configuration is supposed to be used.
    /// </remarks>
    public class GenericWebAPIServerImplementation<StartupType, SettingsInterface, SettingsType>
        where StartupType : AbstractStartup, new()
        where SettingsInterface : class, IISettingsInterface
        where SettingsType : SettingsInterface, new()
    {
        public IConfiguration Configuration { get; private set; } = default;
        public SettingsInterface CurrentSettings { get; private set; } = default;
        public Action OnStart { get; set; } = () => { };
        public Action OnStop { get; set; } = () => { };
        public string ConfigurationFolder { get; set; } = default;
        public string AppSettingsFileName { get; set; } = "AppSettings.json";
        public string AppSettingsSchemaFile { get; set; } = default;
        public GRYLog LogObject { get; private set; }
        public string LogNamespaceForOverhead { get; set; } = "Server";
        public string LogNamespaceForLogMiddleware { get; set; } = "WebServerLog";

        public void Initialize()
        {
            LogObject = GRYLog.Create();//transient log
            if (ConfigurationFolder == default)
            {
                throw new ArgumentNullException($"{nameof(ConfigurationFolder)} is not defined.");
            }
            Utilities.EnsureDirectoryExists(ConfigurationFolder);

            ValidateAppSettings();

            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder
                .SetBasePath(ConfigurationFolder)
                .AddJsonFile(Path.Combine(ConfigurationFolder, AppSettingsFileName), optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();

            CurrentSettings = new SettingsType();
            Configuration.GetSection("Settings").Bind(CurrentSettings);
            LogObject = GRYLog.GetOrCreateAndGet($"{ConfigurationFolder}/Log.configuration", $"{CurrentSettings.GetLogFolder()}/{this.CurrentSettings.GetProgramName()}_{DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss", CultureInfo.InvariantCulture)}.log");//logfile is availabe so use this instead now
        }

        public int Run()
        {
            try
            {
                Log(logObject => logObject.Log($"Started {this.CurrentSettings.GetProgramName()}", LogLevel.Debug), LogNamespaceForOverhead);
                Log(logObject => logObject.Log($"Current environment: {this.CurrentSettings.GetTargetEnvironmentType().GetType().Name}", LogLevel.Information), LogNamespaceForOverhead);

                string protocol = CurrentSettings.WebServerSettings.UseHTTPS ? "https" : "http";
                string domainWithProtocol = $"{protocol}://{CurrentSettings.ServerSettings.Domain}:{CurrentSettings.ServerSettings.Port}";

                WebHostBuilder webHostBuilder = new();
                webHostBuilder.UseEnvironment(this.CurrentSettings.GetTargetEnvironmentType().GetType().Name);
                webHostBuilder.UseUrls(domainWithProtocol);
                webHostBuilder.UseStartup((WebHostBuilderContext webHostBuilderContext) =>
                {
                    return new StartupType()
                    {
                        CurrentSettings = this.CurrentSettings
                    };
                });
                webHostBuilder.UseKestrel(kestrelServerOptions =>
                {
                    X509Certificate2 certificate = null;
                    if (CurrentSettings.WebServerSettings.UseHTTPS)
                    {
                        kestrelServerOptions.ConfigureHttpsDefaults(httpsConnectionAdapterOptions =>
                        {
                            httpsConnectionAdapterOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                        });
                        var pfxFilePath = Utilities.NormalizePath(Path.Combine(ConfigurationFolder, CurrentSettings.WebServerSettings.EncryptionSettings.CertificateFile));
                        var cvertificatePasswordFilePath = Utilities.NormalizePath(Path.Combine(ConfigurationFolder, CurrentSettings.WebServerSettings.EncryptionSettings.CertificatePasswordFile));
                        certificate = new(pfxFilePath, File.ReadAllText(cvertificatePasswordFilePath, new UTF8Encoding(false)));
                        if (this.CurrentSettings.GetTargetEnvironmentType() is Productive && Utilities.IsSelfSIgned(certificate))
                        {
                            Log(logObject => logObject.LogWarning($"The used certificate '{CurrentSettings.WebServerSettings.EncryptionSettings.CertificateFile}' is self-signed. Using self-signed certificates is not recommended in a productive environment."), LogNamespaceForOverhead);
                        }
                    }
                    kestrelServerOptions.Listen(System.Net.IPAddress.Loopback, CurrentSettings.ServerSettings.Port, listenOptions =>
                    {
                        if (CurrentSettings.WebServerSettings.UseHTTPS)
                        {
                            listenOptions.UseHttps(certificate);
                        }
                    });
                    kestrelServerOptions.AddServerHeader = false;
                });

                IWebHost host = webHostBuilder.Build();
                string apiExplorerAddress = $"{domainWithProtocol}/APIExplorer/index.html";
                if (this.CurrentSettings.GetTargetEnvironmentType() is Development)
                {
                    Log(logObject => logObject.Log($"The API-explorer is available under the address '{apiExplorerAddress}'."), LogNamespaceForOverhead);
                }

                OnStart();
                host.Run();
                OnStop();

                return 0;
            }
            catch (Exception exception)
            {
                Log(logObject => logObject.Log(exception, "Fatal error occurred."), LogNamespaceForOverhead);
                return 1;
            }
            finally
            {
                Log(logObject => logObject.Log($"Finished {this.CurrentSettings.GetProgramName()}", LogLevel.Debug), LogNamespaceForOverhead);
            }
        }

        private void ValidateAppSettings()
        {
            string appSettingsFile = Path.Combine(ConfigurationFolder, AppSettingsFileName);
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
                        ICollection<NJsonSchema.Validation.ValidationError> errors = schemaTask.Result.Validate(File.ReadAllText(Path.Combine(ConfigurationFolder, appSettingsFile), new UTF8Encoding(false)));
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
                //TODO create file
            }
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
