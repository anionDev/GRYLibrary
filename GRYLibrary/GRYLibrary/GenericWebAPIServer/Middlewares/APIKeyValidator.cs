using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRYLibrary.Core.GenericWebAPIServer.Services;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public class APIKeyValidator : AbstractMiddleware
    {
        private readonly APIKeyValidatorSettings _APIKeyValidatorSettings;
        public APIKeyValidator(RequestDelegate next, APIKeyValidatorSettings apiKeyValidatorSettings) : base(next)
        {
            _APIKeyValidatorSettings = apiKeyValidatorSettings;
        }

        public override Task Invoke(HttpContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
