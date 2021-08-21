using CryptoCurrencyOnlineTools.Library.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Represents a base-class for middlewares in the context of .NET WebAPI-projects.
    /// </summary>
    public abstract class AbstractMiddleware
    {
        protected readonly RequestDelegate _Next;
        /// <summary>
        /// Creates a new middleware.
        /// </summary>
        public AbstractMiddleware(RequestDelegate next)
        {
            _Next = next;
        }
        public abstract Task Invoke(HttpContext context);
    }
}
