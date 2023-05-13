using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class APIKeyValidatorSettings :IAPIKeyValidatorSettings
    {
        public bool Enabled { get; set; } = true;
        public virtual bool APIKeyIsAuthorized(string apiKey, string method, string route)
        {
            throw new NotSupportedException();
        }

        public virtual bool APIKeyIsRequired(string method, string route)
        {
            return false;
        }

        public virtual (bool provided, string apiKey) TryGetAPIKey(HttpContext context)
        {
            throw new NotSupportedException();
        }

        public class APIKeyFilter :IOperationFilter
        {
            public const string QueryParameterName = "APIKey";
            public static (bool provided, string apiKey) TryGetAPIKey(HttpContext context)
            {
                bool apiKeyIsGiven = context.Request.Query.TryGetValue(APIKeyFilter.QueryParameterName, out StringValues values);
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
                    In = ParameterLocation.Query,
                    Description = "APIKey for method",
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