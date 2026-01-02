using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.MidT.Header
{
    public abstract class HeaderMiddlewareT : AbstractMiddleware
    {
        public abstract void SetHeaders(HttpContext context);
        protected HeaderMiddlewareT(RequestDelegate next) : base(next)
        {
        }

        public override Task Invoke(HttpContext context)
        {
            SetHeaders(context); 
            return this._Next(context);
        }

    }
}
