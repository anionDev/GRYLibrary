using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Blacklist
{
    public class BlacklistConfiguration :IBlacklistConfiguration
    {
        public bool Enabled { get; set; } = true;

        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}