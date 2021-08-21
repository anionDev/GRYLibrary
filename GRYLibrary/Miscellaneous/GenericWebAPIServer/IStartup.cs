using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer
{
    public interface IStartup
    {
        public void ConfigureServices(IServiceCollection services);
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
