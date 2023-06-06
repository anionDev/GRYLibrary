using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public abstract class APIKeyValidatorSettings :IAPIKeyValidatorSettings
    {
        public bool Enabled { get; set; } = true;
        public abstract bool APIKeyIsAuthorized(string apiKey, string method, string route);
        public (bool provided, string apiKey) TryGetAPIKey(HttpContext context)
        {
            return APIKeyFilter.TryGetAPIKey(context);
        }

        public virtual bool APIKeyIsRequired(string method, string route)
        {
            return true;
        }

        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>() { new APIKeyFilter() };
        }
        public class APIKeyFilter :IOperationFilter
        {
            public const string QueryParameterName = "APIKey";
            public static (bool provided, string apiKey) TryGetAPIKey(HttpContext context)
            {
                bool apiKeyIsGiven = context.Request.Headers.TryGetValue(QueryParameterName, out StringValues values);
                if(apiKeyIsGiven)
                {
                    if(values.Count == 1)
                    {
                        return (true, values.First());
                    }
                }
                return (false, null);
            }

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if(operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = QueryParameterName,
                    In = ParameterLocation.Header,
                    Description = "APIKey for authorization",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString(string.Empty)
                    }
                });
            }
        }
    }
}