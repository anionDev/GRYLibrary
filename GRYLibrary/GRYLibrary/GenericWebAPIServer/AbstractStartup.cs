
using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public abstract class AbstractStartup
    {
        public IISettingsInterface CurrentSettings { get; set; }
        public abstract void ConfigureServicesImplementation(IServiceCollection services);
        public abstract void ConfigureImplementation(IApplicationBuilder app);
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IISettingsInterface>((_) => CurrentSettings);
            services.AddControllers();
            //TODO services.AddAntiforgey();
            //TODO on compiletime generate openapi-json-document like services.AddOpenApiDocument() would do
            ConfigureServicesImplementation(services);
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            if (CurrentSettings.Protocol == "https")
            {
                app.UseHsts();
            }
            if (this.CurrentSettings.GetTargetEnvironmentType() is Productive)
            {
                app.UseMiddleware<DDOSProtection>();
                app.UseMiddleware<Obfuscation>();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(); // serve OpenAPI/Swagger documents
                app.UseSwaggerUi3(); // serve Swagger UI
                app.UseReDoc(); // serve ReDoc UI
            }
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionManager>();
            if (this.CurrentSettings.GetTargetEnvironmentType() is QualityCheck || this.CurrentSettings.GetTargetEnvironmentType() is Productive)
            {
                app.UseMiddleware<RequestCounter>();
            }
            app.UseMiddleware<WebApplicationFirewall>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            ConfigureImplementation(app);
        }
    }
}