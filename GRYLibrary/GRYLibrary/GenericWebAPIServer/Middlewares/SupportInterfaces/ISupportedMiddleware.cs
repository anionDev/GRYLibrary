using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.SupportInterfaces
{
    public interface ISupportedMiddleware
    {
        public bool IsEnabled();
        public ISet<IOperationFilter> GetFilter();
    }
}
