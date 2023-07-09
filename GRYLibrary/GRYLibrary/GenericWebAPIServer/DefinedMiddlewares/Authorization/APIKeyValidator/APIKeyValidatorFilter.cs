using GRYLibrary.Core.GenericWebAPIServer.CommonRoutes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Authorization.APIKeyValidator
{
    public class APIKeyValidatorFilter :IOperationFilter
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
            if(!context.MethodInfo.DeclaringType.Assembly.Equals(typeof(CommonRoutesController).Assembly))
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
