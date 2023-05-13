using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    /// <summary>
    /// Represents a blacklist which blocks all requests from blacklisted sources.
    /// </summary>
    public class BlackList :AbstractMiddleware
    {
        private readonly IBlacklistProvider _BlacklistProvider;
        /// <inheritdoc/>
        public BlackList(RequestDelegate next, IBlacklistProvider blacklistProvider) : base(next)
        {
            this._BlacklistProvider = blacklistProvider;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            //TODO
            return this._Next(context);
        }
    }
}