using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    /// <summary>
    /// Contains common extensions for middlewares
    /// </summary>
    public static class MiddlewareExtension
    {
        /// <summary>Configure the application to use <see cref="DDOSProtection"/>.</summary>
        public static IApplicationBuilder UseDDOSProtection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DDOSProtection>();
        }
        /// <summary>Configure the application to use <see cref="WebApplicationFirewall"/>.</summary>
        public static IApplicationBuilder UseWebApplicationFirewall(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebApplicationFirewall>();
        }
        /// <summary>Configure the application to use <see cref="Obfuscation"/>.</summary>
        public static IApplicationBuilder UseObfuscation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Obfuscation>();
        }
        /// <summary>Configure the application to use <see cref="Log"/>.</summary>
        public static IApplicationBuilder UseLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Log>();
        }
        /// <summary>Configure the application to use <see cref="ExceptionManager"/>.</summary>
        public static IApplicationBuilder UseExceptionManager(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionManager>();
        }
        /// <summary>Configure the application to use <see cref="RequestCounter"/>.</summary>
        public static IApplicationBuilder UseRequestCounter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCounter>();
        }
    }
}
