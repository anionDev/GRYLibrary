using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Blacklist
{
    /// <summary>
    /// Represents a blacklist which blocks all requests from blacklisted sources.
    /// </summary>
    public class BlacklistMiddleware :AbstractMiddleware
    {
        private readonly IBlacklistConfiguration _BlacklistProvider;
        /// <inheritdoc/>
        public BlacklistMiddleware(RequestDelegate next, IBlacklistConfiguration blacklistProvider) : base(next)
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