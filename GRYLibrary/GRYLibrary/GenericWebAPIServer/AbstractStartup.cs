
using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public abstract class AbstractStartup
    {
        public IISettingsInterface CurrentSettings { get; set; }       
       public abstract void ConfigureServicesImplementation(IServiceCollection services);
        public abstract void ConfigureImplementation(IApplicationBuilder app);
        public  void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //TODO: .AddAntiforgey()
            //TODO do this on compiletime: services.AddOpenApiDocument(); // add OpenAPI v3 document
            ConfigureServicesImplementation(services);
        }
        public  void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseHsts();
            if (this.CurrentSettings.GetEnvironment() is Productive)
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
            if (this.CurrentSettings.GetEnvironment() is QualityCheck || this.CurrentSettings.GetEnvironment() is Productive)
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