using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public abstract class AuthenticationMiddleware : AbstractMiddleware
    {
        public const string IsAuthenticatedInformationName = "IsAuthenticated";
        private readonly IAuthenticationConfiguration _AuthenticationConfiguration;
        protected AuthenticationMiddleware(RequestDelegate next, IAuthenticationConfiguration authenticationConfiguration) : base(next)
        {
            this._AuthenticationConfiguration = authenticationConfiguration;
        }
        public virtual bool AuthenticationIsRequired(HttpContext context)
        {
            foreach (var routesWhereUnauthenticatedAccessIsAllowed in this._AuthenticationConfiguration.RoutesWhereUnauthenticatedAccessIsAllowed)
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
            if (this.TryGetAuthorizeAttribute(context, out AuthorizeAttribute _))
            {
                return true;
            }
            return false;
        }

        public abstract bool TryGetAuthentication(HttpContext context, out ClaimsPrincipal principal);
        public override Task Invoke(HttpContext context)
        {
            if (!this.IsAuthenticatedInternal(context) && this.AuthenticationIsRequired(context))
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
            if (this.TryGetAuthentication(context, out ClaimsPrincipal principal))
            {
                context.User = principal;
                result = true;
            }
            else
            {
                result = false;
            }
            context.Items[IsAuthenticatedInformationName] = result;
            return result;
        }
    }
}
