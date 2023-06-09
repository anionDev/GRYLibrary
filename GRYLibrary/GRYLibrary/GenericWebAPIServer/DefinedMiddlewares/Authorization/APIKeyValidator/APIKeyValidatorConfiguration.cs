using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authorization.APIKeyValidator
{
    public abstract class APIKeyValidatorConfiguration :IAPIKeyValidatorConfiguration
    {
        public bool Enabled { get; set; } = true;
      
        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>() { new APIKeyValidatorFilter() };
        }
    }
}