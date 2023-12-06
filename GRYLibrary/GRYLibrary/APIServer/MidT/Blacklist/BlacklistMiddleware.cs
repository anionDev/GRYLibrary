using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Blacklist
{
    /// <summary>
    /// Represents a blacklist which blocks all requests from blacklisted sources.
    /// </summary>
    public abstract class BlacklistMiddleware : AbstractMiddleware
    {
        /// <inheritdoc/>
        public BlacklistMiddleware(RequestDelegate next) : base(next)
        {
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            //TODO
            return this._Next(context);
        }
    }
}