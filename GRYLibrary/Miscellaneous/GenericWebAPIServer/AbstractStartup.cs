
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer
{
    public abstract class AbstractStartup/*TODO implement Microsoft.AspNetCore.Hosting.IStartup*/
    {
        public AbstractStartup()
        {

        }
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesImplementation(services);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureImplementation(app, env);
        }
        public abstract void ConfigureServicesImplementation(IServiceCollection services);
        public abstract void ConfigureImplementation(IApplicationBuilder app, IWebHostEnvironment env);
    }
}