
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer
{
    public interface IStartup/*: Microsoft.AspNetCore.Hosting.IStartup*/
    {
        public void ConfigureServices(IServiceCollection services);
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    }
}