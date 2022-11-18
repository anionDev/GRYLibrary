
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public abstract class AbstractStartup 
    {
        public AbstractStartup()
        {
        }
        public abstract void ConfigureServicesImplementation(IServiceCollection services);
        public abstract void ConfigureImplementation(IApplicationBuilder app);

        void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SettingsInterface>((_) => new SettingsType());//TODO get this generics from implementing class
            services.AddControllers();
            //TODO: .AddAntiforgey()
            //TODO do this on compiletime: services.AddOpenApiDocument(); // add OpenAPI v3 document
            ConfigureServicesImplementation(services);
        }

        void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<DDOSProtection>();
            app.UseMiddleware<Obfuscation>();
            app.UseMiddleware<ExceptionManager>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<RequestCounter>();
            app.UseMiddleware<WebApplicationFirewall>();
            ConfigureImplementation(app);
        }
    }
}