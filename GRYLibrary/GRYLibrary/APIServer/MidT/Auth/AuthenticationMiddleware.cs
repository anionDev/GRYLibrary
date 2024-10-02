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
            foreach (string routesWhereUnauthenticatedAccessIsAllowed in this._AuthenticationConfiguration.RoutesWhereUnauthenticatedAccessIsAllowed)
            {
                if (new Regex(routesWhereUnauthenticatedAccessIsAllowed).IsMatch(context.Request.Path.Value))
                {
                    return this.Return(false, context);
                }
            }
            if (this.TryGetAuthenticateAttribute(context, out AuthenticateAttribute _))
            {
                return this.Return(true, context);
            }
            if (this.TryGetAuthorizeAttribute(context, out AuthorizeAttribute _))
            {
                return this.Return(true, context);
            }

            return this.Return(false, context);
        }

        private bool Return(bool authenticationIsRequired, HttpContext context)
        {
            context.Items[IsAuthenticatedInformationName] = authenticationIsRequired;
            return authenticationIsRequired;
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
            if (this.TryGetAuthentication(context, out ClaimsPrincipal principal))
            {
                context.User = principal;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
