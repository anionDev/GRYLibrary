
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
            if (this.CurrentSettings.GetTargetEnvironmentType() is Productive)
            {
                app.UseMiddleware<DDOSProtection>();
                app.UseMiddleware<Obfuscation>();
            }
            app.UseMiddleware<BlackList>();
            if (this.CurrentSettings.WebServerSettings.UseHTTPS)
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            if (!(this.CurrentSettings.GetTargetEnvironmentType() is Productive))
            {
                app.UseDeveloperExceptionPage();

                /*
                TODO
                app.UseOpenApi(c => {
                    c.Path = "OpenAPI.json";
                });
                 */

                /*
                //TODO
                app.UseSwaggerUi3(c =>
                {
                    c.RoutePrefix=WebServerSettings.APIExplorerSubRouter;
                }); 
                */

            }
            if (CurrentSettings.WebServerSettings.BasePath != null)
            {
                app.UsePathBase(CurrentSettings.WebServerSettings.BasePath);
            }
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionManager>();
            if (this.CurrentSettings.GetTargetEnvironmentType() is QualityCheck || this.CurrentSettings.GetTargetEnvironmentType() is Productive)
            {
                app.UseMiddleware<RequestCounter>();
            }
            app.UseMiddleware<WebApplicationFirewall>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            ConfigureImplementation(app);
        }
    }
}