using GRYLibrary.Core.APIServer.Services.Aut.Prov;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public abstract class AuthenticationMiddleware : AbstractMiddleware
    {
        public const string IsAuthenticatedInformationName = "IsAuthenticated";
        public const string UserIdInformationName = "UserId";
        public const string CurrentlyUsedAccessTokenInformationName = "CurrentlyUsedAccessToken";
        private readonly IAuthenticationConfiguration _AuthenticationConfiguration;
        private readonly IDictionary<string, IAuthenticationProvider> _AuthenticationProvider;
        private readonly IAuthenticationService _AuthenticationService;
        protected AuthenticationMiddleware(RequestDelegate next, IAuthenticationConfiguration authenticationConfiguration, IAuthenticationService authenticationService) : base(next)
        {
            this._AuthenticationConfiguration = authenticationConfiguration;
            this._AuthenticationProvider = this.GetAllAvailableAuthenticationProvider(authenticationConfiguration.AuthentificationMethods);
            _AuthenticationConfiguration = authenticationConfiguration;
        }

        private IDictionary<string, IAuthenticationProvider> GetAllAvailableAuthenticationProvider(IDictionary<string, IAuthenticationProviderConfiguration> externalAuthentificationMethods)
        {
            var result = new Dictionary<string, IAuthenticationProvider>();
            foreach (var method in externalAuthentificationMethods)
            {
                result[method.Key] = method.Value.CreateProvider();
            }
            return result;
        }

        public virtual bool AuthenticationIsRequired(HttpContext context)
        {
            foreach (string routesWhereUnauthenticatedAccessIsAllowed in this._AuthenticationConfiguration.RoutesWhereUnauthenticatedAccessIsAllowed)
            {
                if (new Regex(routesWhereUnauthenticatedAccessIsAllowed).IsMatch(context.Request.Path.Value))
                {
                    return false;
                }
            }
            if (this.TryGetAuthenticateAttribute(context, out AuthenticateAttribute _))
            {
                return true;
            }
            if (this.TryGetAuthorizeAttribute(context, out AuthorizeAttribute _))//this check is here because authorization can not be checked when authentication is not given. this implies that if authorization if rewuired, then authentication is required too.
            {
                return true;
            }
            return false;
        }

        public override Task Invoke(HttpContext context)
        {
            bool authenticationIsRequired = this.AuthenticationIsRequired(context);
            bool isAuthenticatedInternal = this.IsAuthenticatedInternal(context);
            if (authenticationIsRequired & !isAuthenticatedInternal)
            {
                throw new BadRequestException(StatusCodes.Status401Unauthorized);
            }
            else
            {
                return this._Next(context);
            }
        }

        public virtual bool IsAuthenticatedInternal(HttpContext context)
        {
            bool result = false;

            foreach (var authenticationProvider in this._AuthenticationProvider)
            {
                if (authenticationProvider.Value.IsApplicable(context))
                {
                    if (authenticationProvider.Value.TryGetAuthentication(context, out string accessToken))
                    {
                        if (_AuthenticationService.AccessTokenIsValid(accessToken))
                        {
                            context.User = _AuthenticationService.GetPrincipal(accessToken);
                            result = true;
                            context.Items[CurrentlyUsedAccessTokenInformationName] = accessToken;
                            context.Items[UserIdInformationName] = context.User.Claims.Where(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First().Value;
                            context.Items[IsAuthenticatedInformationName] = true;
                            return true;
                        }
                    }
                }
            }
            context.Items[IsAuthenticatedInformationName] = false;
            return false;

        }

    }
}
