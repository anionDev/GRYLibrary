using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Blacklist
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