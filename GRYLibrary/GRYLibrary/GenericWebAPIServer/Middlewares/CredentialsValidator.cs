using GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static GRYLibrary.Core.GenericWebAPIServer.Middlewares.CredentialsValidator;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public class CredentialsValidator :AbstractMiddleware
    {
        private readonly ICredentialsValidatorSettings _APIKeyValidatorSettings;
        public CredentialsValidator(RequestDelegate next, ICredentialsValidatorSettings apiKeyValidatorSettings) : base(next)
        {
            this._APIKeyValidatorSettings = apiKeyValidatorSettings;
        }
        public class AccessMethod
        {
            private readonly HttpContext _Context;
            private readonly string _Method;
            private readonly string _Route;

            public AccessMethod(HttpContext context, string method, string route)
            {
                this._Context = context;
                this._Method = method;
                this._Route = route;
            }

            public bool IsProvided()
            {
                throw new NotImplementedException();
            }
            public bool IsRequired()
            {
                throw new NotImplementedException();
            }
            public bool IsAuthenticated()
            {
                throw new NotImplementedException();
            }

            public bool IsAuthorized()
            {
                throw new NotImplementedException();
            }
        }

        public override Task Invoke(HttpContext context)
        {
            string method = context.Request.Method;
            string route = context.Request.Path;

            AccessMethod userAuthenticationAccess = GetUserAccessMethod(context, method, route);
            AccessMethod apiKey = GetAPIKeyAccessMethod(context, method, route);

            IEnumerable<AccessMethod> accessMethods = new List<AccessMethod>() { userAuthenticationAccess, apiKey };

            var required = accessMethods.Where(accessMethod => accessMethod.IsRequired());
            bool accessAllowed;
            if(required.Count() == 0)
            {
                accessAllowed = true;
            }
            else
            {
                accessMethods = accessMethods.Where(accessMethod => accessMethod.IsProvided());
                if(accessMethods.Count() == 0)
                {
                    context.Response.StatusCode = 401;

                    accessAllowed = false;
                }

                accessMethods = accessMethods.Where(accessMethod => accessMethod.IsAuthenticated());
                if(!accessMethods.Any())
                {
                    context.Response.StatusCode = 401;
                    accessAllowed = false;
                }

                accessMethods = accessMethods.Where(accessMethod => accessMethod.IsAuthorized());
                if(!accessMethods.Any())
                {
                    context.Response.StatusCode = 403;
                    accessAllowed = false;
                }
                else if(accessMethods.Count() == 1)
                {
                    accessAllowed = true;
                }
                else
                {
                    context.Response.StatusCode = 409;
                    accessAllowed = false;
                }
            }
            if(accessAllowed)
            {
                if(accessMethods.Count() == 1)
                {
                    context.Items[nameof(AccessMethod)] = accessMethods.First();
                }
                return this._Next(context);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public AccessMethod GetAPIKeyAccessMethod(HttpContext context, string method, string route)
        {
            AccessMethod am = new AccessMethod(context, method, route);
            return am;
        }

        public AccessMethod GetUserAccessMethod(HttpContext context, string method, string route)
        {
            AccessMethod am = new AccessMethod(context, method, route);
            return am;
        }
    }
}