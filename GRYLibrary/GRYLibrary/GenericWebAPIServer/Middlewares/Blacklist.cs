using GRYLibrary.Core.GenericWebAPIServer.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a blacklist which blocks all requests from blacklisted sources.
    /// </summary>
    public class BlackList : AbstractMiddleware
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
            return _Next(context);
        }
    }
}
