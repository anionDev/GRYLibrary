using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Mid.M01APIK
{
    public class APIKeyValidatorFilter : IOperationFilter
    {
        public const string HeaderName = "X-Authorization";
        public static (bool provided, string apiKey) TryGetAPIKey(HttpContext context)
        {
            bool apiKeyIsGiven = context.Request.Headers.TryGetValue(HeaderName, out StringValues values);
            if (apiKeyIsGiven)
            {
                if (values.Count == 1)
                {
                    string apiKey = values.First();
                    return (true, apiKey);
                }
            }
            return (false, null);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }
            if (context.MethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false).Length != 0)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = HeaderName,
                    Description = "API-Key",
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema() { Type = "string" },
                    Required = true,
                    //Example = new OpenApiString("some example")
                });
            }
        }
    }
}
