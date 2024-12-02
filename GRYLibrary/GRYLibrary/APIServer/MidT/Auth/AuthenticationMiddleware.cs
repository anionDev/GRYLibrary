using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
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
        protected AuthenticationMiddleware(RequestDelegate next, IAuthenticationConfiguration authenticationConfiguration) : base(next)
        {
            this._AuthenticationConfiguration = authenticationConfiguration;
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

        public abstract bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal, out string accessToken);
        public override Task Invoke(HttpContext context)
        {
            if (this.AuthenticationIsRequired(context) && !this.IsAuthenticatedInternal(context))
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
            bool result;
            if (this.TryGetAuthentication(context, out ClaimsPrincipal principal, out string accessToken))
            {
                context.User = principal;
                result = true;
            }
            else
            {
                result = false;
            }
            context.Items[IsAuthenticatedInformationName] = result;
            if (result)
            {
                context.Items[CurrentlyUsedAccessTokenInformationName] = accessToken;
                context.Items[UserIdInformationName] = principal.Claims.Where(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First().Value;
            }
            return result;
        }
    }
}
