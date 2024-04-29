﻿using GRYLibrary.Core.APIServer.MidT.Exception;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;

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
            else if (exception is AggregateException aggregateException2)
            {
                if (aggregateException2.InnerExceptions == null)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (aggregateException2.InnerExceptions.Count == 0)
                    {
                        throw new NotImplementedException();
                    }
                    else if (aggregateException2.InnerExceptions.Count == 1)
                    {
                        exceptionForFormatting = aggregateException2.InnerExceptions[0];
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                exceptionForFormatting = exception;
            }
            if (exceptionForFormatting is BadRequestException badHttpRequestException)
            {
                context.Response.StatusCode = (int)badHttpRequestException.HTTPStatusCode;
            }
            else
            {
                this._GeneralLogger.LogException(exceptionForFormatting, "Error while processing request");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            (string ContentType, string bodyContent) = this.GetResponceContent(context.Response.StatusCode, context, exceptionForFormatting);
            context.Response.ContentType = ContentType;
            context.Response.WriteAsync(bodyContent).Wait();
        }
        public virtual (string ContentType, string bodyContent) GetResponceContent(int httpStatusCode, HttpContext context, Exception exception)
        {
            return (null, string.Empty);
        }
    }
}
