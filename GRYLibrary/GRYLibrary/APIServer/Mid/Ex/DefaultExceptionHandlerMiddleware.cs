using GRYLibrary.Core.APIServer.MidT.Exception;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace GRYLibrary.Core.APIServer.Mid.Ex
{
    public class DefaultExceptionHandlerMiddleware : ExceptionManagerMiddleware
    {
        private readonly IGeneralLogger _GeneralLogger;
        public DefaultExceptionHandlerMiddleware(RequestDelegate next, IGeneralLogger logger) : base(next)
        {
            this._GeneralLogger = logger;
        }

        protected override void HandleException(HttpContext context, Exception exception)
        {
            Exception exceptionForFormatting;
            if (exception == null)
            {
                throw new NotImplementedException();
            }
            else if (exception is AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions == null)
                {
                    exceptionForFormatting = new InternalAlgorithmException("No inner exception given.");
                }
                else
                {
                    if (aggregateException.InnerExceptions.Count == 0)
                    {
                        exceptionForFormatting = new InternalAlgorithmException("No inner exceptions given.");
                    }
                    else if (aggregateException.InnerExceptions.Count == 1)
                    {
                        exceptionForFormatting = aggregateException.InnerExceptions[0];
                    }
                    else
                    {
                        exceptionForFormatting = aggregateException;
                    }
                }
            }
            else
            {
                exceptionForFormatting = exception;
            }

            if (exceptionForFormatting is BadRequestException badHttpRequestException)
            {
                context.Response.StatusCode = badHttpRequestException.HTTPStatusCode;
                this.Log("Bad request.", context, exceptionForFormatting, (uint)context.Response.StatusCode);
            }
            else if (exceptionForFormatting is InvalidCredentialsException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                this.Log("Invalid credentials", context, exceptionForFormatting, (uint)context.Response.StatusCode);
            }
            else if (exceptionForFormatting is NotAuthorizedException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                this.Log("Not authorized", context, exceptionForFormatting, (uint)context.Response.StatusCode);
            }
            else if (exceptionForFormatting is NotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                this.Log("Not found", context, exceptionForFormatting, (uint)context.Response.StatusCode);
            }
            else if (exceptionForFormatting is InternalAlgorithmException)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                this.Log("Internal error", context, exceptionForFormatting, (uint)context.Response.StatusCode);
            }
            else if (exceptionForFormatting is DependencyNotAvailableException)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                this.Log("Dependency not available", context, exceptionForFormatting, (uint)context.Response.StatusCode);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                this.Log("Error while processing request", context, exceptionForFormatting, (uint)context.Response.StatusCode);
            }
            (string ContentType, string bodyContent) = this.GetExceptionResponceContent(context.Response.StatusCode, context, exceptionForFormatting);
            context.Response.ContentType = ContentType;
            context.Response.WriteAsync(bodyContent).Wait();
        }

        private void Log(string technicalReason, HttpContext context, Exception exception, uint statuscode)
        {
            this._GeneralLogger.Log($"Request {context.Items["RequestId"]} resulted in statuscode {statuscode}. Technical reason: {technicalReason}", exception, Microsoft.Extensions.Logging.LogLevel.Trace);
        }

        public virtual (string ContentType, string bodyContent) GetExceptionResponceContent(int httpStatusCode, HttpContext context, Exception exception)
        {
            return (null, string.Empty);
        }

        public virtual (string ContentType, string bodyContent) GetNotFoundResponseContent(HttpContext context)
        {
            return (null, string.Empty);
        }
    }
}
